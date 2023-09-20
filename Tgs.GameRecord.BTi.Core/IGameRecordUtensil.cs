using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTi.Core.Models;

namespace Tgs.GameRecord.BTi.Core;

public interface IGameRecordUtensil
{
    RecordInfo SettlementConvertRecord(
        GameHistorySettled source,
        CancellationToken cancellationToken = default);

    RecordInfo UnsettledConvertRecord(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default);
}
