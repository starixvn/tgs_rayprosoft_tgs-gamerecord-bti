namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

public record PullRecordUnsettledCommand(
    Uri DataApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
