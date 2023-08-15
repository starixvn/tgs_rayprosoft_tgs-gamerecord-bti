using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;

    public HandleRecordCommandService(
        IGameRecordHandleClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {

        var status = GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        if (command.GameRecord.Status != GameHistoryStatus.Opened)
        {
            status = GameRecordStatus.Closed;
            clientStatus =
            command.GameRecord.Status switch
            {
                GameHistoryStatus.Draw => GameRecordClientStatus.Draw,
                GameHistoryStatus.Declined => GameRecordClientStatus.Draw,
                GameHistoryStatus.Canceled => GameRecordClientStatus.Void,
                GameHistoryStatus.Won => GameRecordClientStatus.Win,
                GameHistoryStatus.HalfWon => GameRecordClientStatus.Win,
                GameHistoryStatus.HalfLost => GameRecordClientStatus.Loss,
                _ => (command.GameRecord.WinLoss > 0) ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss
            };
        }

        await _client
        .MessageAsync(
            gameProviderCode: command.GameRecord.GameProviderCode,
            externalRoundId: command.GameRecord.ExternalRoundId,
            externalGameId: command.GameRecord.ExternalGameId,
            playerCode: command.GameRecord.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: command.GameRecord.ExternalCreatedOn,
            externalClosedOn: command.GameRecord.ExternalClosedOn,
            bet: command.GameRecord.Bet,
            validBet: command.GameRecord.ValidBet,
            settle: command.GameRecord.Settle,
            commission: command.GameRecord.Commission,
            turnover: command.GameRecord.Turnover,
            playerWinLoss: command.GameRecord.WinLoss,
            additionalData: command.GameRecord.AdditionalData,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    }
}
