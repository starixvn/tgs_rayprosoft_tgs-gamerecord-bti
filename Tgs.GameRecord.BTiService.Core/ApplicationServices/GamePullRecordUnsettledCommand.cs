using Tgs.GameInterface.BTiService.Protos;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

public record GamePullRecordUnsettledCommand(IEnumerable<GameHistoryUnsettled> Records);
