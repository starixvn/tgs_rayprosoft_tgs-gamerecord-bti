using Microsoft.Extensions.Options;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class HandleRecordCommandService(
    IOptions<GameRecordInfoOptions> options,
    IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = WagerStates.Running;
        var cashoutState = CashoutStates.None;
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var bet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Info.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;

        if (command.Info.IsClosed)
        {
            externalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset();
            settle = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Info.ValidBet.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
            wagerState = command.Info.Status switch
            {
                GameHistoryStatus.Draw => WagerStates.Draw,
                GameHistoryStatus.Declined => WagerStates.Reject,
                GameHistoryStatus.Canceled => WagerStates.Refund,
                GameHistoryStatus.Won => WagerStates.Win,
                GameHistoryStatus.HalfWon => WagerStates.HalfWin,
                GameHistoryStatus.HalfLost => WagerStates.HalfLose,
                GameHistoryStatus.Cashout => WagerStates.Refund,
                GameHistoryStatus.Lost => WagerStates.Lose,
                _ => (winLoss > 0) ? WagerStates.Win : WagerStates.Lose
            };
            cashoutState = (command.Info.Status == GameHistoryStatus.Cashout) ? CashoutStates.Cashout : CashoutStates.None;
        }

        return client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Info.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: cashoutState,
            resettleState: ResettleStates.None,
            externalRoundId: command.Info.ExternalWagerId,
            externalWagerId: command.Info.ExternalWagerId,
            externalGameId: _gameRecordInfo.GameExternalId,
            externalCreatedOn: command.Info.ExternalCreatedOn.ToDateTimeOffset(),
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: 0.00m,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: command.Info.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
