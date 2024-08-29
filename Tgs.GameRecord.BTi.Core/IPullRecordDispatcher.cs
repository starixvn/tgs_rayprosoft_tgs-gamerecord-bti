using Tgs.GameRecord.BTi.Core.ApplicationServices;

namespace Tgs.GameRecord.BTi.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
