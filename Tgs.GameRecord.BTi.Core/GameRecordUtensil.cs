using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTi.Core.Models;

namespace Tgs.GameRecord.BTi.Core;

internal class GameRecordUtensil : IGameRecordUtensil
{
    private readonly IUtensil _utensil;

    public GameRecordUtensil(
        IUtensil utensil)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public RecordInfo SettlementConvertRecord(
        GameHistorySettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderCode = _utensil.GetGameProviderCode();
        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        DateTimeOffset? externalClosedOn = source.IsClosed ? source.ExternalClosedOn.ToDateTimeOffset() : null;
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = source.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0;
        var turnover = source.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = source.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            ExternalGameId: _utensil.GetGameExternalId(),
            PlayerCode: source.PlayerCode,
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

    public RecordInfo UnsettledConvertRecord(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderCode = _utensil.GetGameProviderCode();
        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        DateTimeOffset? externalClosedOn = null;
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0;
        var commission = 0;
        var turnover = 0;
        var winLoss = 0;

        return new RecordInfo(
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            ExternalGameId: _utensil.GetGameExternalId(),
            PlayerCode: source.PlayerCode,
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
