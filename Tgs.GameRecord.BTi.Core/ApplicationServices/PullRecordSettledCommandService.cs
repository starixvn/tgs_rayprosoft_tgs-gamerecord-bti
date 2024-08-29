using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordSettledCommandService(
	ILogger<PullRecordSettledCommandService> logger,
	IGameInterfaceBTiClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordSettledCommand>
{
	public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
	{
		int page = 0;
		int pageLimit = 1000;

		IEnumerable<GameWagerInfo> infos;

		do
		{
			infos = await client.GameWagerSettledAsync(
			   dataApiUrl: command.DataApiUrl,
			   agentUserName: command.AgentUserName,
			   agentPassword: command.AgentPassword,
			   startTime: command.StartTime.UtcDateTime,
			   endTime: command.EndTime.UtcDateTime,
			   page: page,
			   pageLimit: pageLimit,
			   cancellationToken: cancellationToken)
			   .ToArrayAsync(cancellationToken: cancellationToken)
			   .ConfigureAwait(false)
			   ?? Enumerable.Empty<GameWagerInfo>();

			if (infos.Any())
			{
				foreach (var info in infos)
				{
					try
					{
						await service.ExecuteAsync(
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

			page++;
		} while (infos.Any());
	}
}
