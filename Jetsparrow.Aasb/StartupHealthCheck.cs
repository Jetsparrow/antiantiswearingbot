using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jetsparrow.Aasb;
public class StartupHealthCheck : IHealthCheck
{
    Aasb Bot { get; }

    public StartupHealthCheck(Aasb bot)
    {
        Bot = bot;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (Bot.Started)
            return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
        else
            return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
    }
}