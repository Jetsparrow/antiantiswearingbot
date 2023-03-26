using Microsoft.Extensions.Diagnostics.HealthChecks;

using Jetsparrow.Aasb.Services;

namespace Jetsparrow.Aasb.Health;
public class StartupHealthCheck : IHealthCheck
{
    AntiAntiSwearingBot Bot { get; }

    public StartupHealthCheck(AntiAntiSwearingBot bot)
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