namespace Tgs.GameRecord.BTi.ViewModels;

public record PullRecordViewModel(
    Uri TokenApiUrl,
    Uri OpenBetsApiUrl,
    Uri HistoryBetsPagingApiUrl,
    string AgentUserName,
    string AgentPassword,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
