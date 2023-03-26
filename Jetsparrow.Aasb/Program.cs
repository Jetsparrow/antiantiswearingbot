using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Jetsparrow.Aasb;
using Jetsparrow.Aasb.Services;
using Jetsparrow.Aasb.Health;

var builder = WebApplication.CreateBuilder();
Console.WriteLine("Configuring...");

var cfg = builder.Configuration;
var svc = builder.Services;

svc.AddOptions<SearchDictionarySettings>().BindConfiguration("SearchDictionary").ValidateDataAnnotations();
svc.AddOptions<TelegramSettings>().BindConfiguration("Telegram");
svc.AddOptions<UnbleeperSettings>().BindConfiguration("Unbleeper");


svc.AddHealthChecks().AddCheck<StartupHealthCheck>("Startup");
svc.AddSingleton<SearchDictionary>();
svc.AddSingleton<Unbleeper>();
svc.AddHostedSingleton<AntiAntiSwearingBot>();

Console.WriteLine("Building...");
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseEndpoints(cfg =>
{
    cfg.MapHealthChecks("/health");
    cfg.MapHealthChecks("/health/verbose", new()
    {
        ResponseWriter = HealthUtils.WriteHealthCheckResponse
    });
});

Console.WriteLine("Running...");
app.Run();
