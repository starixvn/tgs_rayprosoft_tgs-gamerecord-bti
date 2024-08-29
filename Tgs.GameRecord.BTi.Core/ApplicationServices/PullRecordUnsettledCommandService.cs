using System.Text.Json;
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

		if (!infos.Any())
		{
			return;
		}

		foreach (var info in infos)
		{
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
				logger.LogError(ex, "{logError}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						Data = new
						{
							info.PlayerCode,
							info.ExternalWagerId,
							ExternalCreatedOn = info.ExternalCreatedOn.ToDateTimeOffset(),
							ExternalClosedOn = info.ExternalClosedOn.ToDateTimeOffset()
						},
						Message = "An exception occurred in this game wager."
					}));
			}
		}
	}
}
