using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTiService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.WorkerServices;

internal class GameHistoryWorkerService : ICommandService<GameHistoryWorkerCommand>
{
    private readonly IGameInterfaceBTiClient _client;
    private readonly ILogger<GameHistoryWorkerService> _logger;
    private readonly ICommandService<GamePullRecordUnsettledCommand> _unsettledService;
    private readonly ICommandService<GamePullRecordSettledCommand> _settledService;

    public GameHistoryWorkerService(
        IGameInterfaceBTiClient client,
        ILogger<GameHistoryWorkerService> logger,
        ICommandService<GamePullRecordUnsettledCommand> unsettledService,
        ICommandService<GamePullRecordSettledCommand> settledService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unsettledService = unsettledService ?? throw new ArgumentNullException(nameof(unsettledService));
        _settledService = settledService ?? throw new ArgumentNullException(nameof(settledService));
    }

    public async ValueTask ExecuteAsync(GameHistoryWorkerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameHistoryUnsettleds = await _client
                    .GameHistoryUnsettledAsync(
                        tokenApiUrl: command.TokenApiUrl,
                        openBetsApiUrl: command.OpenBetsApiUrl,
                        agentUserName: command.AgentUserName,
                        agentPassword: command.AgentPassword,
                        startTime: command.StartTime,
                        endTime: command.EndTime,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                gameHistoryUnsettleds);

            await _unsettledService
                .ExecuteAsync(
                    command: new GamePullRecordUnsettledCommand(
                        Records: gameHistoryUnsettleds),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:Bti處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }

        try
        {
            var gameHistorySettleds = await _client
                    .GameHistorySettledAsync(
                        tokenApiUrl: command.TokenApiUrl,
                        historyBetsPagingApiUrl: command.HistoryBetsPagingApiUrl,
                        agentUserName: command.AgentUserName,
                        agentPassword: command.AgentPassword,
                        startTime: command.StartTime,
                        endTime: command.EndTime,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Action:GameHistorySettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                gameHistorySettleds);

            await _settledService
                .ExecuteAsync(
                    command: new GamePullRecordSettledCommand(
                        Records: gameHistorySettleds),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:Bti處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
