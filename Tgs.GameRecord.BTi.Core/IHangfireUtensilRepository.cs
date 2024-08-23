using Tgs.GameRecord.BTi.Core.JobApplicationServices;

namespace Tgs.GameRecord.BTi.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
