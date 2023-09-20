namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

public record PullRecordUnsettledCommand(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
