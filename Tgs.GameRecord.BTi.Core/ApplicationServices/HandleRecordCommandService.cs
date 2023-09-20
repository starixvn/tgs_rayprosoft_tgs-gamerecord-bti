using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;

    public HandleRecordCommandService(
        IGameRecordHandleClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = WagerStates.Running;
        var cashoutState = CashoutStates.None;
        if (command.GameRecord.Status != GameHistoryStatus.Opened)
        {
            wagerState =
            command.GameRecord.Status switch
            {
                GameHistoryStatus.Draw => WagerStates.Draw,
                GameHistoryStatus.Declined => WagerStates.Reject,
                GameHistoryStatus.Canceled => WagerStates.Refund,
                GameHistoryStatus.Won => WagerStates.Win,
                GameHistoryStatus.HalfWon => WagerStates.HalfWin,
                GameHistoryStatus.HalfLost => WagerStates.HalfLose,
                GameHistoryStatus.Cashout => WagerStates.Refund,
                GameHistoryStatus.Lost => WagerStates.Lose,
                _ => (command.GameRecord.WinLoss > 0) ? WagerStates.Win : WagerStates.Lose
            };
            cashoutState = (command.GameRecord.Status == GameHistoryStatus.Cashout) ? CashoutStates.Cashout : CashoutStates.None;
        }

        return _client
            .MessageAsync(
            gameProviderCode: command.GameRecord.GameProviderCode,
            playerCode: command.GameRecord.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: cashoutState,
            resettleState: ResettleStates.None,
            externalRoundId: command.GameRecord.ExternalRoundId,
            externalWagerId: command.GameRecord.ExternalRoundId,
            externalGameId: command.GameRecord.ExternalGameId,
            externalCreatedOn: command.GameRecord.ExternalCreatedOn,
            externalClosedOn: command.GameRecord.ExternalClosedOn,
            externalPayoffOn: command.GameRecord.ExternalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: command.GameRecord.Bet,
            validBet: command.GameRecord.ValidBet,
            settle: command.GameRecord.Settle,
            commission: command.GameRecord.Commission,
            turnover: command.GameRecord.Turnover,
            winLoss: command.GameRecord.WinLoss,
            additionalData: command.GameRecord.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
