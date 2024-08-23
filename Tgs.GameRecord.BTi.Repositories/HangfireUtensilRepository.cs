using Tgs.GameRecord.BTi.Core;
using Tgs.GameRecord.BTi.Core.JobApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.BTi.Repositories;

internal class HangfireUtensilRepository(
	ICommandService<JobCheckCommand> jobCheckService,
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensilRepository
{
	public Task JobCheckAsync()
		=> jobCheckService.ExecuteAsync(new()).AsTask();

	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
