using System.Text.Json;
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
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Message = "An exception occurred during the unsettled processing of this Bti."
				}));
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
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Message = "An exception occurred during settlement processing for this Bti."
				}));
		}
	}
}
