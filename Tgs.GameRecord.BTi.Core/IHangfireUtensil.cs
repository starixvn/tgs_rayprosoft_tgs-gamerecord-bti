using Tgs.GameRecord.BTi.Core.JobApplicationServices;

namespace Tgs.GameRecord.BTi.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
