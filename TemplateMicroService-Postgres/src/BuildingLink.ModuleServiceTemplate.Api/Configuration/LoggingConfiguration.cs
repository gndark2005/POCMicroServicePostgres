using BuildingLink.ModuleServiceTemplate.Api.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingLink.ModuleServiceTemplate.Api.Configuration
{
    public static class LoggingConfiguration
    {
        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .Enrich.WithThreadId()
                    .Enrich.FromLogContext()
                    .Enrich.With(services.GetService<IdentityEnricher>())
                    .ReadFrom.Configuration(hostingContext.Configuration);
            });
            return hostBuilder;
        }
    }
}
