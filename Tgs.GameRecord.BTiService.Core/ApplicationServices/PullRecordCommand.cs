namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

public record PullRecordCommand(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
