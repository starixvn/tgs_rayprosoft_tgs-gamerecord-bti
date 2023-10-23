using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.UnitTest.ApplicationServices;

public class PullRecordSettledCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordSettledCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceBTiClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordSettledCommand(
            DataApiUrl: new("https://data-api.442hattrick.com/"),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: DateTimeOffset.Parse("2023-05-29T00:00:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-05-30T00:00:00+08:00"));

        var service = new PullRecordSettledCommandService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service.ExecuteAsync(command);

        _ = await fakeClient
            .Received(1)
            .GameWagerSettledAsync(
            dataApiUrl: Arg.Is(command.DataApiUrl),
            agentUserName: Arg.Is(command.AgentUserName),
            agentPassword: Arg.Is(command.AgentPassword),
            startTime: Arg.Is(command.StartTime),
            endTime: Arg.Is(command.EndTime),
            page: Arg.Any<int>(),
            pageLimit: Arg.Any<int>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(cancellationToken: Arg.Any<CancellationToken>());
    }
}
