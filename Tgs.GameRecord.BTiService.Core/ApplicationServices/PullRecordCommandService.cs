using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly IGameInterfaceBTiClient _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<ManualHandleRecordCommand> _manualService;
    private readonly ICommandService<ScheduleHandleRecordCommand> _scheduleService;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        IGameInterfaceBTiClient client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<ScheduleHandleRecordCommand> scheduleService,
        ICommandService<ManualHandleRecordCommand> manualService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _manualService = manualService ?? throw new ArgumentNullException(nameof(manualService));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameRecords = await _client
                    .GameHistoryUnsettledAsync(
                        tokenApiUrl: command.TokenApiUrl,
                        openBetsApiUrl: command.OpenBetsApiUrl,
                        agentUserName: command.AgentUserName,
                        agentPassword: command.AgentPassword,
                        startTime: command.StartTime.UtcDateTime,
                        endTime: command.EndTime.UtcDateTime,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

            foreach (var gameRecord in gameRecords)
            {
                _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                try
                {
                    var record = await _gameRecordUtensil
                        .UnsettledConvertRecordAsync(
                            gameRecord,
                            cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. Record:{record}", DateTime.Now.ToString("o"), record);

                    if (command.Source == ScheduleFormSource.Schedule)
                    {
                        await _scheduleService
                            .ExecuteAsync(
                                command: new(GameRecord: record),
                                cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await _manualService
                            .ExecuteAsync(
                                command: new(GameRecord: record),
                                cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Unsettlement Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:Bti處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }

        try
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
                        .ConfigureAwait(false);

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
                                gameRecord,
                                cancellationToken)
                            .ConfigureAwait(false);

                        _logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. Record:{record}", DateTime.Now.ToString("o"), record);

                        if (command.Source == ScheduleFormSource.Schedule)
                        {
                            await _scheduleService
                                .ExecuteAsync(
                                    command: new(GameRecord: record),
                                    cancellationToken: cancellationToken)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            await _manualService
                                .ExecuteAsync(
                                    command: new(GameRecord: record),
                                    cancellationToken: cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Settlement Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                    }
                }

                page++;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:Bti處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
