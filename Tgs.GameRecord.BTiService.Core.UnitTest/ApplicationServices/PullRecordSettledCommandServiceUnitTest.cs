using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTiService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.UnitTest.ApplicationServices;

public class PullRecordSettledCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordSettledCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceBTiClient>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordUtensil>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordSettledCommand(
            TokenApiUrl: new("https://asia-east2-bq-data-api.cloudfunctions.net/"),
            HistoryBetsPagingApiUrl: new("https://get-history-bets-paging-d5nlg5buca-de.a.run.app/"),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: DateTimeOffset.Parse("2023-05-29T00:00:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-05-30T00:00:00+08:00"));

        var service = new PullRecordSettledCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameHistorySettledAsync(
            tokenApiUrl: Arg.Is(command.TokenApiUrl),
            historyBetsPagingApiUrl: Arg.Is(command.HistoryBetsPagingApiUrl),
            agentUserName: Arg.Is(command.AgentUserName),
            agentPassword: Arg.Is(command.AgentPassword),
            startTime: Arg.Is(command.StartTime.UtcDateTime),
            endTime: Arg.Is(command.EndTime.UtcDateTime),
            page: Arg.Any<int>(),
            pageLimit: Arg.Any<int>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
