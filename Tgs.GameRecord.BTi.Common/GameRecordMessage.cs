namespace Tgs.GameRecord.BTi.Common;

public record GameRecordMessage(
    Uri DataApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
