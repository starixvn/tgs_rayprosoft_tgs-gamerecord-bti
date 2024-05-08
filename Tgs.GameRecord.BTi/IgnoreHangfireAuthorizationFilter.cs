using Hangfire.Dashboard;

namespace Tgs.GameRecord.BTi;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
