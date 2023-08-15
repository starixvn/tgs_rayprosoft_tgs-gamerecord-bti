using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTiService.Core.Models;

namespace Tgs.GameRecord.BTiService.Core;

public interface IGameRecordUtensil
{
    RecordInfo SettlementConvertRecord(
        GameHistorySettled source,
        CancellationToken cancellationToken = default);

    RecordInfo UnsettledConvertRecord(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default);
}
