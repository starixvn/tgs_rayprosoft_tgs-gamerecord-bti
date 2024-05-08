using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordUnsettledCommandService(
	ILogger<PullRecordUnsettledCommandService> logger,
	IGameInterfaceBTiClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordUnsettledCommand>
{
	public async ValueTask ExecuteAsync(PullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
	{
		var infos = await client
			.GameWagerUnsettledAsync(
			dataApiUrl: command.DataApiUrl,
			agentUserName: command.AgentUserName,
			agentPassword: command.AgentPassword,
			startTime: command.StartTime.UtcDateTime,
			endTime: command.EndTime.UtcDateTime,
			cancellationToken: cancellationToken)
			.ToArrayAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerInfo>();

		logger.LogDebug("LogOn:{logOn} Message: Game Wager Unsettled. Infos:{infos}", DateTime.Now.ToString("o"), infos);

		if (!infos.Any())
		{
			return;
		}

		foreach (var info in infos)
		{
			logger.LogDebug("LogOn:{logOn} Message: Game Wager Unsettled. GameWager:{info}", DateTime.Now.ToString("o"), info);
			try
			{
				await service
					.ExecuteAsync(
					command: new(Info: info),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({info})", DateTime.Now.ToString("o"), info);
			}
		}
	}
}
