using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var fakeUnsettledService = Substitute.For<ICommandService<PullRecordUnsettledCommand>>();
        var fakeSettledService = Substitute.For<ICommandService<PullRecordSettledCommand>>();

        var command = new PullRecordCommand(
            TokenApiUrl: new("https://asia-east2-bq-data-api.cloudfunctions.net/"),
            OpenBetsApiUrl: new("https://get-open-bets-d5nlg5buca-de.a.run.app/"),
            HistoryBetsPagingApiUrl: new("https://get-history-bets-paging-d5nlg5buca-de.a.run.app/"),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: DateTimeOffset.Parse("2023-05-29T00:00:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-05-30T00:00:00+08:00"));

        var service = new PullRecordCommandService(
            fakeLogger,
            fakeUnsettledService,
            fakeSettledService);

        await service.ExecuteAsync(command);

        await fakeUnsettledService
            .Received(1)
            .ExecuteAsync(
            Arg.Any<PullRecordUnsettledCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeSettledService
            .Received(1)
            .ExecuteAsync(
            Arg.Any<PullRecordSettledCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
