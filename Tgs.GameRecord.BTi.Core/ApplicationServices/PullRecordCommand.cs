namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

public record PullRecordCommand(
    Uri DataApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
