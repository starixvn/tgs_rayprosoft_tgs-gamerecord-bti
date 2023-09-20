using Microsoft.Extensions.Logging;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

internal class PullRecordUnsettledCommandService : ICommandService<PullRecordUnsettledCommand>
{
    private readonly IGameInterfaceBTiClient _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<PullRecordUnsettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordUnsettledCommandService(
        ILogger<PullRecordUnsettledCommandService> logger,
        IGameInterfaceBTiClient client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
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
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryUnsettled>();

        _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

        if (!gameRecords.Any())
        {
            return;
        }

        foreach (var gameRecord in gameRecords)
        {
            _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
            try
            {
                var record = _gameRecordUtensil
                    .UnsettledConvertRecord(
                    source: gameRecord,
                    cancellationToken: cancellationToken);

                _logger.LogDebug("LogOn:{logOn} Message: GameRecord Unsettlement. Record:{record}", DateTime.Now.ToString("o"), record);

                await _service
                    .ExecuteAsync(
                    command: new(GameRecord: record),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
            }
        }
    }
}
