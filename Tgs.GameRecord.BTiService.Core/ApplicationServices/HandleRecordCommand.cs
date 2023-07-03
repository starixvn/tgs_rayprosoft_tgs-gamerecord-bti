using Tgs.GameRecord.BTiService.Core.Models;

namespace Tgs.GameRecord.BTiService.Core.ApplicationServices;

public record HandleRecordCommand(
    RecordInfo GameRecord);
