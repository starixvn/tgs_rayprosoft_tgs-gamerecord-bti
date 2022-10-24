using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

internal class GamePullRecordSettledCommandService : ICommandService<GamePullRecordSettledCommand>
{
    private readonly ILogger<GamePullRecordSettledCommandService> _logger;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;

    public GamePullRecordSettledCommandService(
        ILogger<GamePullRecordSettledCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync().ConfigureAwait(false);
        var gameId = await _utensil.GetGameIdAsync(gameProviderId, _gameRecordInfo.GameExternalId).ConfigureAwait(false);
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil.GetPlayerAsync(record.PlayerCode).ConfigureAwait(false);

            if (playerModel == null)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, gameProviderId).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            GameRoundStatus status = (record.IsClosed) ? GameRoundStatus.Closed : GameRoundStatus.Open;
            GameRoundClientStatus clientStatus = (!record.IsClosed) ? GameRoundClientStatus.Open :
                record.Status switch
                {
                    GameHistoryStatus.Draw => GameRoundClientStatus.Draw,
                    GameHistoryStatus.Declined => GameRoundClientStatus.Draw,
                    GameHistoryStatus.Canceled => GameRoundClientStatus.Void,
                    GameHistoryStatus.Won => GameRoundClientStatus.Win,
                    GameHistoryStatus.HalfWon => GameRoundClientStatus.Win,
                    GameHistoryStatus.HalfLost => GameRoundClientStatus.Loss,
                    _ => (record.PlayerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss
                };

            DateTimeOffset? externalClosedOn = (record.IsClosed) ? record.ExternalClosedOn.ToDateTimeOffset() : null;

            var data = new GameRecordMessage(
                ExternalRoundId: record.ExternalRoundId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId,
                ExternalGameId: _gameRecordInfo.GameExternalId,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: status,
                ClientStatus: clientStatus,
                ExternalCreatedOn: record.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn: externalClosedOn,
                BetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.ValidBet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.Settle,
                Commission: 0,
                Turnover: record.Turnover,
                PlayerWinLoss: record.PlayerWinLoss,
                AdditionalData: record.AdditionalData,
                Success: success);

            var subject = _gameRecordInfo.MongoGlobPattern;

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, data);

            await _messageSender
                    .PublishAsync(
                        subject: subject,
                        data: data.ToBson(),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
