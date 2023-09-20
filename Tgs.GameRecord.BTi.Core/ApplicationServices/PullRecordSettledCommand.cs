namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

public record PullRecordSettledCommand(
    Uri TokenApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
