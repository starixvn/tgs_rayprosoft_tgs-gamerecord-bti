using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	ICommandService<PullRecordUnsettledCommand> unsettledService,
	ICommandService<PullRecordSettledCommand> settledService) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			var unsettledCommand = new PullRecordUnsettledCommand(
				DataApiUrl: command.DataApiUrl,
				AgentUserName: command.AgentUserName,
				AgentPassword: command.AgentPassword,
				StartTime: command.StartTime,
				EndTime: command.EndTime);

			await unsettledService
				.ExecuteAsync(
				command: unsettledCommand,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:Bti處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
		}

		try
		{
			var settledCommand = new PullRecordSettledCommand(
				DataApiUrl: command.DataApiUrl,
				AgentUserName: command.AgentUserName,
				AgentPassword: command.AgentPassword,
				StartTime: command.StartTime,
				EndTime: command.EndTime);

			await settledService
				.ExecuteAsync(
				command: settledCommand,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:Bti處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
		}
	}
}
