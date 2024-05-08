using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = Handle.Common.Protos.BetState.Running;
		var cashoutState = Handle.Common.Protos.CashoutState.None;
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
			betState = command.Info.Status switch
			{
				GameHistoryStatus.Draw => Handle.Common.Protos.BetState.Draw,
				GameHistoryStatus.Declined => Handle.Common.Protos.BetState.Void,
				GameHistoryStatus.Canceled => Handle.Common.Protos.BetState.Cancel,
				GameHistoryStatus.Won => Handle.Common.Protos.BetState.Win,
				GameHistoryStatus.HalfWon => Handle.Common.Protos.BetState.HalfWin,
				GameHistoryStatus.HalfLost => Handle.Common.Protos.BetState.HalfLoss,
				GameHistoryStatus.Cashout => Handle.Common.Protos.BetState.Refund,
				GameHistoryStatus.Lost => Handle.Common.Protos.BetState.Loss,
				_ => (winLoss > 0) ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss
			};
			cashoutState = (command.Info.Status == GameHistoryStatus.Cashout) ? Handle.Common.Protos.CashoutState.Cashout : Handle.Common.Protos.CashoutState.None;
		}

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.Info.PlayerCode,
			ExternalRoundId = command.Info.ExternalWagerId,
			ExternalBetId = command.Info.ExternalWagerId,
			ExternalGameId = _gameRecordInfo.GameExternalId,
			ExternalCreatedOn = command.Info.ExternalCreatedOn,
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = cashoutState,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.Info.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}
