using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.BTi.Common;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Controllers;

[Route("[controller]")]
[ApiController]
public class BTiController : ControllerBase
{
	/// <summary>
	/// Executions the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost]
	public ValueTask ExecutionAsync(
		[FromServices] ICommandService<PullRecordCommand> service,
		[FromBody] BTiGameRecordMessage source)
		=> service
		.ExecuteAsync(
			command: new PullRecordCommand(
				DataApiUrl: source.DataApiUrl,
				AgentUserName: source.AgentUserName,
				AgentPassword: source.AgentPassword,
				StartTime: source.StartTime,
				EndTime: source.EndTime),
			cancellationToken: HttpContext.RequestAborted);
}
