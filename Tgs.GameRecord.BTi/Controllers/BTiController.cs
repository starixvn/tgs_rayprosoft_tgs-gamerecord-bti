using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.GameRecord.BTi.ViewModels;
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
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "dataApiUrl": "https://data-api.442hattrick.com/",
	///             "agentUserName": "PegaGaming_GPIRMB",
	///             "agentPassword": "IM60Ho7Y4B9C",
	///             "startTime": "2023-05-29T00:00:00+08:00",
	///             "endTime": "2023-05-30T00:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost]
	public ValueTask ExecutionAsync(
		[FromServices] ICommandService<PullRecordCommand> service,
		[FromBody] PullRecordViewModel source)
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
