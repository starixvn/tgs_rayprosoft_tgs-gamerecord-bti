using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.UnitTest.ApplicationServices;

public class PullRecordUnsettledCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordUnsettledCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceBTiClient>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordUtensil>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordUnsettledCommand(
            TokenApiUrl: new("https://asia-east2-bq-data-api.cloudfunctions.net/"),
            OpenBetsApiUrl: new("https://get-open-bets-d5nlg5buca-de.a.run.app/"),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: DateTimeOffset.Parse("2023-05-29T00:00:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-05-30T00:00:00+08:00"));

        var service = new PullRecordUnsettledCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service.ExecuteAsync(command);

        _ = await fakeClient
            .Received(1)
            .GameHistoryUnsettledAsync(
            tokenApiUrl: Arg.Is(command.TokenApiUrl),
            openBetsApiUrl: Arg.Is(command.OpenBetsApiUrl),
            agentUserName: Arg.Is(command.AgentUserName),
            agentPassword: Arg.Is(command.AgentPassword),
            startTime: Arg.Is(command.StartTime.UtcDateTime),
            endTime: Arg.Is(command.EndTime.UtcDateTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(cancellationToken: Arg.Any<CancellationToken>());
    }
}
