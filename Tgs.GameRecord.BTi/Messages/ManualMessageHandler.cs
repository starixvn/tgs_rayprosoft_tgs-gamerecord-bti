using System.Text;
using System.Text.Json;
using Tgs.GameRecord.BTi.Common;
using Tgs.GameRecord.BTi.Core;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.BTi.Messages;

public class ManualMessageHandler(
	ILogger<ManualMessageHandler> logger,
	IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<BTiGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.DataApiUrl,
					message.AgentUserName,
					message.AgentPassword,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This message receiving handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordCommand(
				DataApiUrl: message.DataApiUrl,
				AgentUserName: message.AgentUserName,
				AgentPassword: message.AgentPassword,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
