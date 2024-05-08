namespace Tgs.GameRecord.BTi.ViewModels;

public record PullRecordViewModel(
	Uri DataApiUrl,
	string AgentUserName,
	string AgentPassword,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
