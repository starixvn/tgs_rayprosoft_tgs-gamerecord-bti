using Tgs.GameInterface.BTiService.Protos;

namespace Tgs.GameRecord.BTi.Core.Models;

public record RecordInfo(
    string GameProviderCode,
    string ExternalRoundId,
    string ExternalGameId,
    string PlayerCode,
    GameHistoryStatus Status,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset? ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss,
    string AdditionalData);
