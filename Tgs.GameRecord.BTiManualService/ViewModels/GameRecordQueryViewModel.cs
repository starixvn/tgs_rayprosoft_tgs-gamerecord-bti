namespace Tgs.GameRecord.BTiManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTime StartTime,
    DateTime EndTime);
