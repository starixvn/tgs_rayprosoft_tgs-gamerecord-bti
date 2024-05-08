namespace Tgs.GameRecord.BTi.Core.ApplicationServices;

public record PullRecordSettledCommand(
	Uri DataApiUrl,
	string AgentUserName,
	string AgentPassword,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
