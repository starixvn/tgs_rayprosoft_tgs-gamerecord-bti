namespace Tgs.GameRecord.BTi.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri DataApiUrl,
	string AgentUserName,
	string AgentPassword);
