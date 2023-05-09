using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTiService.Core.Models;

namespace Tgs.GameRecord.BTiService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GameHistorySettled source,
        CancellationToken cancellationToken = default);

    public ValueTask<RecordInfo> UnsettledConvertRecordAsync(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default);
}
