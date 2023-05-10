using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRecord.BTiService.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiService.Core.UnitTest;

public class ScheduleFormDispatcherUnitTest
{
    [Fact]
    public async Task Exception_Occurs()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "BTI";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new BTiParameter
        {
            TokenApiUrl = "https://asia-east2-bq-data-api.cloudfunctions.net/",
            OpenBetsApiUrl = "https://get-open-bets-d5nlg5buca-de.a.run.app/",
            HistoryBetsPagingApiUrl = "https://get-history-bets-paging-d5nlg5buca-de.a.run.app/",
            AgentUserName = "PegaGaming_GPIRMB",
            AgentPassword = "IM60Ho7Y4B9C",
            StartTime = DateTimeOffset.Parse("2023-05-29T00:00:00+08:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-05-30T00:00:00+08:00").ToTimestamp()
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.bti.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        _ = fakeScheduleFormLock
            .LockingAsync(
                id: Arg.Is(id))
            .Returns(true);

        var scheduleCommand = new PullRecordCommand(
            TokenApiUrl: new(parameter.TokenApiUrl),
            OpenBetsApiUrl: new(parameter.OpenBetsApiUrl),
            HistoryBetsPagingApiUrl: new(parameter.HistoryBetsPagingApiUrl),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset(),
            Source: scheduleFormReply.Source);

        _ = fakePullRecord
           .ExecuteAsync(
               command: Arg.Is(scheduleCommand),
               cancellationToken: Arg.Any<CancellationToken>())
           .Throws<Exception>();

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakePullRecord);

        await dispatcher
            .ExecuteAsync(
            gameProviderCode: gameProviderCode,
                id: id)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .ScheduleFormProcessingAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(0)
            .ScheduleFormCompleteAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .ScheduleFormFaileAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                status: Arg.Any<string>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        _ = await fakeScheduleFormLock
            .Received(1)
            .UnlockAsync(
                id: Arg.Is(id))
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task Normal_Process_Schedule()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "BTI";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new BTiParameter
        {
            TokenApiUrl = "https://asia-east2-bq-data-api.cloudfunctions.net/",
            OpenBetsApiUrl = "https://get-open-bets-d5nlg5buca-de.a.run.app/",
            HistoryBetsPagingApiUrl = "https://get-history-bets-paging-d5nlg5buca-de.a.run.app/",
            AgentUserName = "PegaGaming_GPIRMB",
            AgentPassword = "IM60Ho7Y4B9C",
            StartTime = DateTimeOffset.Parse("2023-05-29T00:00:00+08:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-05-30T00:00:00+08:00").ToTimestamp()
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.bti.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        _ = fakeScheduleFormLock
            .LockingAsync(
                id: Arg.Is(id))
            .Returns(true);

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakePullRecord);

        await dispatcher
            .ExecuteAsync(
                 gameProviderCode: gameProviderCode,
                id: id)
            .ConfigureAwait(false);

        var scheduleCommand = new PullRecordCommand(
            TokenApiUrl: new(parameter.TokenApiUrl),
            OpenBetsApiUrl: new(parameter.OpenBetsApiUrl),
            HistoryBetsPagingApiUrl: new(parameter.HistoryBetsPagingApiUrl),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset(),
            Source: scheduleFormReply.Source);

        await fakePullRecord
            .Received(1)
            .ExecuteAsync(
                command: Arg.Is(scheduleCommand),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .ScheduleFormProcessingAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .ScheduleFormCompleteAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        _ = await fakeScheduleFormLock
            .Received(1)
            .UnlockAsync(
                id: Arg.Is(id))
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task Schedule_Form_State_Is_Not_Queued()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "BTI";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new BTiParameter
        {
            TokenApiUrl = "https://asia-east2-bq-data-api.cloudfunctions.net/",
            OpenBetsApiUrl = "https://get-open-bets-d5nlg5buca-de.a.run.app/",
            HistoryBetsPagingApiUrl = "https://get-history-bets-paging-d5nlg5buca-de.a.run.app/",
            AgentUserName = "PegaGaming_GPIRMB",
            AgentPassword = "IM60Ho7Y4B9C",
            StartTime = DateTimeOffset.Parse("2023-05-29T00:00:00+08:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-05-30T00:00:00+08:00").ToTimestamp()
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.bti.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Processing,
            Status = $"{ScheduleFormState.Processing}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakePullRecord);

        await dispatcher
            .ExecuteAsync(
                 gameProviderCode: gameProviderCode,
                id: id)
            .ConfigureAwait(false);

        _ = await fakeScheduleFormLock
            .Received(0)
            .LockingAsync(
                id: Arg.Is(id))
            .ConfigureAwait(false);

        var scheduleCommand = new PullRecordCommand(
            TokenApiUrl: new(parameter.TokenApiUrl),
            OpenBetsApiUrl: new(parameter.OpenBetsApiUrl),
            HistoryBetsPagingApiUrl: new(parameter.HistoryBetsPagingApiUrl),
            AgentUserName: "PegaGaming_GPIRMB",
            AgentPassword: "IM60Ho7Y4B9C",
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset(),
            Source: scheduleFormReply.Source);

        await fakePullRecord
            .Received(0)
            .ExecuteAsync(
                command: Arg.Is(scheduleCommand),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        _ = await fakeScheduleFormLock
            .Received(0)
            .UnlockAsync(
                id: Arg.Is(id))
            .ConfigureAwait(false);
    }
}
