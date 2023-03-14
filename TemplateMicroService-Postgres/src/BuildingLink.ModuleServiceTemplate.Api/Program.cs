using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BuildingLink.ModuleServiceTemplate.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingLink.ModuleServiceTemplate.Api
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (args.Any(arg => arg == "/reseed"))
            {
                Console.WriteLine("host.CleanData()");
                host.CleanData();

                Console.WriteLine("host.SeedData()");
                host.SeedData();
            }
            else if (args.Any(arg => arg == "/seed"))
            {
                Console.WriteLine("host.SeedData()");
                host.SeedData();
            }
            else
            {
                Console.WriteLine("host.Run()");
                host.Run();
            }
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
