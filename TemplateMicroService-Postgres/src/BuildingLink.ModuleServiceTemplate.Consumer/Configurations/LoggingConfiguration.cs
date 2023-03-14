using NewRelic.LogEnrichers.Serilog;
using Serilog;
using Serilog.Events;
using System.Text.Json;

namespace BuildingLink.ModuleServiceTemplate.Consumer.Configurations;

public static class LoggingConfiguration
{
    public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich
                .FromLogContext()
                .ReadFrom.Configuration(hostingContext.Configuration);
        });
        return hostBuilder;
    }
}