namespace Tgs.GameRecord.BTi.Core.JobApplicationServices;

public record JobExecuteCommand(
    string GameProviderCode,
    string Name,
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword);
