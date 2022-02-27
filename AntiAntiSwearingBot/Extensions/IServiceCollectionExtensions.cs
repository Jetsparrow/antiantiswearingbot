using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AntiAntiSwearingBot;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedSingleton<TService>(this IServiceCollection isc) where TService : class, IHostedService
    {
        return isc.AddSingleton<TService>().AddHostedService(svc => svc.GetRequiredService<TService>());
    }
}
