using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

internal class PullRecordSettledCommandService : ICommandService<PullRecordSettledCommand>
{
    private readonly IGameInterfaceBTiClient _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<PullRecordSettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _scheduleService;

    public PullRecordSettledCommandService(
        ILogger<PullRecordSettledCommandService> logger,
        IGameInterfaceBTiClient client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<HandleRecordCommand> scheduleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
    }

    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        int page = 0;
        int pageLimit = 1000;

        while (true)
        {
            var gameRecords = await _client
                .GameHistorySettledAsync(
                tokenApiUrl: command.TokenApiUrl,
                historyBetsPagingApiUrl: command.HistoryBetsPagingApiUrl,
                agentUserName: command.AgentUserName,
                agentPassword: command.AgentPassword,
                startTime: command.StartTime.UtcDateTime,
                endTime: command.EndTime.UtcDateTime,
                page: page,
                pageLimit: pageLimit,
                cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? Enumerable.Empty<GameHistorySettled>();

            _logger.LogInformation("LogOn:{logOn} | Action:GameHistorySettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                JsonSerializer.Serialize(gameRecords));

            if (!gameRecords.Any())
            {
                break;
            }

            _logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. GameRecords:{payGameRecords}", DateTime.Now.ToString("o"), gameRecords);

            foreach (var gameRecord in gameRecords)
            {
                _logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                try
                {
                    var record = await _gameRecordUtensil
                        .SettlementConvertRecordAsync(
                        source: gameRecord,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. Record:{record}", DateTime.Now.ToString("o"), record);

                    await _scheduleService
                        .ExecuteAsync(
                        command: new(GameRecord: record),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Settlement Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                }
            }

            page++;
        }
    }
}
