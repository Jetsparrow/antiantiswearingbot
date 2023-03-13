using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AntiAntiSwearingBot;

var builder = WebApplication.CreateBuilder();

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment.EnvironmentName;
    config.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
    config.AddJsonFile($"secrets.{env}.json", optional: true, reloadOnChange: true);
});

var cfg = builder.Configuration;
var svc = builder.Services;

svc.Configure<SearchDictionarySettings>(cfg.GetSection("SearchDictionary"));
svc.Configure<TelegramSettings>(cfg.GetSection("Telegram"));
svc.Configure<UnbleeperSettings>(cfg.GetSection("Unbleeper"));

svc.AddHealthChecks().AddCheck<StartupHealthCheck>("Startup");
svc.AddHostedSingleton<SearchDictionary>();
svc.AddSingleton<Unbleeper>();
svc.AddHostedSingleton<Aasb>();

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseEndpoints(cfg =>
{
    cfg.MapHealthChecks("/health");
});
app.Run();
