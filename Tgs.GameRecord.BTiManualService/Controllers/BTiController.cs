using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.BTiManualService.ViewModels;
using Tgs.GameRecord.BTiService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTiManualService.Controllers;

[ApiController]
[Route("[controller]")]
public class BTiController : ControllerBase
{
    /// <summary>
    /// Games the history by sports book asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://ex-api-demo-yy.568win.com",
    ///             "companyKey": "49ED28A987624CA0A3F834C3B8A69AAB",
    ///             "agent": "STAG_GPI_CNY",
    ///             "startTime": "2022-09-22T17:00:00+08:00",
    ///             "endTime": "2022-09-22T17:30:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask GameHistoryBySportsBookAsync(
        [FromServices] ICommandService<GameHistoryWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new GameHistoryWorkerCommand(
            TokenApiUrl: model.TokenApiUrl,
            OpenBetsApiUrl: model.OpenBetsApiUrl,
            HistoryBetsPagingApiUrl: model.HistoryBetsPagingApiUrl,
            AgentUserName: model.AgentUserName,
            AgentPassword: model.AgentPassword,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
