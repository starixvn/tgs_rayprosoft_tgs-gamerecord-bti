using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<PullRecordSettledCommand> _settledService;
    private readonly ICommandService<PullRecordUnsettledCommand> _unsettledService;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        ICommandService<PullRecordUnsettledCommand> unsettledService,
        ICommandService<PullRecordSettledCommand> settledService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unsettledService = unsettledService ?? throw new ArgumentNullException(nameof(unsettledService));
        _settledService = settledService ?? throw new ArgumentNullException(nameof(settledService));
    }

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

            await _unsettledService
                .ExecuteAsync(
                command: unsettledCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:Bti處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }

        try
        {
            var settledCommand = new PullRecordSettledCommand(
                DataApiUrl: command.DataApiUrl,
                AgentUserName: command.AgentUserName,
                AgentPassword: command.AgentPassword,
                StartTime: command.StartTime,
                EndTime: command.EndTime);

            await _settledService
                .ExecuteAsync(
                command: settledCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:Bti處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
