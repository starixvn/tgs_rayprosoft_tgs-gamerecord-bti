using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Core.UnitTest.ApplicationServices;

public class PullRecordUnsettledCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<PullRecordUnsettledCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceBTiClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new PullRecordUnsettledCommand(
			DataApiUrl: new("https://data-api.442hattrick.com/"),
			AgentUserName: "PegaGaming_GPIRMB",
			AgentPassword: "IM60Ho7Y4B9C",
			StartTime: DateTimeOffset.Parse("2023-05-29T00:00:00+08:00"),
			EndTime: DateTimeOffset.Parse("2023-05-30T00:00:00+08:00"));

		var service = new PullRecordUnsettledCommandService(
			fakeLogger,
			fakeClient,
			fakeService);

		await service.ExecuteAsync(command);

		_ = await fakeClient
			.Received(1)
			.GameWagerUnsettledAsync(
			dataApiUrl: Arg.Is(command.DataApiUrl),
			agentUserName: Arg.Is(command.AgentUserName),
			agentPassword: Arg.Is(command.AgentPassword),
			startTime: Arg.Is(command.StartTime),
			endTime: Arg.Is(command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>())
			.ToArrayAsync(cancellationToken: Arg.Any<CancellationToken>());
	}
}
