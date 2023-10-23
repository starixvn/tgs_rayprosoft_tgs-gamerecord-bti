using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordSettledCommandService : ICommandService<PullRecordSettledCommand>
{
    private readonly IGameInterfaceBTiClient _client;
    private readonly ILogger<PullRecordSettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _scheduleService;

    public PullRecordSettledCommandService(
        ILogger<PullRecordSettledCommandService> logger,
        IGameInterfaceBTiClient client,
        ICommandService<HandleRecordCommand> scheduleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
    }

    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        int page = 0;
        int pageLimit = 1000;

        IEnumerable<GameWagerInfo> infos;

        do
        {
            infos = await _client
                .GameWagerSettledAsync(
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

            _logger.LogDebug("LogOn:{logOn} Message: Game Wager Settled. GameWagers:{infos}", DateTime.Now.ToString("o"), infos);

            if (infos.Any())
            {
                foreach (var info in infos)
                {
                    _logger.LogDebug("LogOn:{logOn} Message: Game Wager Settled. GameWager:{info}", DateTime.Now.ToString("o"), info);
                    try
                    {
                        await _scheduleService.ExecuteAsync(
                            command: new(Info: info),
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({info})", DateTime.Now.ToString("o"), info);
                    }
                }
            }

            page++;
        } while (infos.Any());
    }
}
