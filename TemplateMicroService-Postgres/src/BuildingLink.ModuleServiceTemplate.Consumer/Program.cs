using System.Diagnostics.CodeAnalysis;
using BuildingLink.ModuleServiceTemplate.Consumer;
using BuildingLink.ModuleServiceTemplate.Consumer.Configurations;

namespace BuildingLink.ModuleServiceTemplate.Consumer
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) => config.AddEnvironmentVariables())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging();
    }
}
