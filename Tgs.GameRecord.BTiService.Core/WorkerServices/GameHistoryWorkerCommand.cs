namespace Tgs.GameRecord.BTiService.Core.WorkerServices;

public record GameHistoryWorkerCommand(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTime StartTime,
    DateTime EndTime);
