namespace Tgs.GameRecord.BTiService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri TokenApiUrl,
    Uri HistoryBetsPagingApiUrl,
    Uri OpenBetsApiUrl,
    string AgentUserName,
    string AgentPassword);
