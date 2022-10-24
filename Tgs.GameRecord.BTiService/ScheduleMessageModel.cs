namespace Tgs.GameRecord.BTiService;

public record ScheduleMessageModel(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword);
