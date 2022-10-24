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
    ///             "tokenApiUrl": "https://asia-east2-bq-data-api.cloudfunctions.net/",
    ///             "openBetsApiUrl": "https://get-open-bets-d5nlg5buca-de.a.run.app/",
    ///             "historyBetsPagingApiUrl": "https://get-history-bets-paging-d5nlg5buca-de.a.run.app/",
    ///             "agentUserName": "PegaGaming_GPIRMB",
    ///             "agentPassword": "IM60Ho7Y4B9C",
    ///             "startTime": "2022-10-24T00:00:00+08:00",
    ///             "endTime": "2022-10-25T00:00:00+08:00
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
