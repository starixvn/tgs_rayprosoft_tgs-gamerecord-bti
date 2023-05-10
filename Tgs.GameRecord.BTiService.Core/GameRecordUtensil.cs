using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTiService.Core.Models;

namespace Tgs.GameRecord.BTiService.Core;

internal class GameRecordUtensil : IGameRecordUtensil
{
    private readonly IUtensil _utensil;

    public GameRecordUtensil(
        IUtensil utensil)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GameHistorySettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var player = await _utensil
            .GetPlayerAsync(
                playerCode: source.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        DateTimeOffset? externalClosedOn = source.IsClosed ? source.ExternalClosedOn.ToDateTimeOffset() : null;
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = source.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0;
        var turnover = source.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = source.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            GameId: gameId,
            ExternalGameId: _utensil.GetGameExternalId(),
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Status: source.Status,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: winLoss,
            AdditionalData: source.AdditionalData);
    }

    public async ValueTask<RecordInfo> UnsettledConvertRecordAsync(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var player = await _utensil
            .GetPlayerAsync(
                playerCode: source.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        DateTimeOffset? externalClosedOn = null;
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0;
        var commission = 0;
        var turnover = 0;
        var winLoss = 0;

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            GameId: gameId,
            ExternalGameId: _utensil.GetGameExternalId(),
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Status: GameHistoryStatus.Opened,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: winLoss,
            AdditionalData: source.AdditionalData);
    }
}
