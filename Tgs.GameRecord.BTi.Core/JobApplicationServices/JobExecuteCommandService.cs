using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.BTi.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.BTi.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	private readonly double _intervalMinutes = Math.Clamp(
			value: options.Value.IntervalMinutes,
			min: options.Value.IntervalMinutesMinimum,
			max: options.Value.IntervalMinutesMax);

	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var endTime = DateTimeOffset.Parse(currentTime.ToString("yyyy-MM-ddTHH:mm:00.000z"));
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var message = new BTiGameRecordMessage
		{
			DataApiUrl = command.DataApiUrl,
			AgentUserName = command.AgentUserName,
			AgentPassword = command.AgentPassword,
			StartTime = startTime,
			EndTime = endTime
		};

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = BTiNatsConst.SchedulePublish,
				Data = new
				{
					message.DataApiUrl,
					message.AgentUserName,
					message.AgentPassword,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This message publishing handler."
			}));

		return messageSender
			.PublishAsync(
			subject: BTiNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(message),
			cancellationToken: cancellationToken);
	}
}
