using Tgs.GameInterface.BTiService.Protos;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

public record GamePullRecordSettledCommand(IEnumerable<GameHistorySettled> Records);
