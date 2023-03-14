using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BuildingLink.ModuleServiceTemplate.Tests.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationManager
    {
        public static IConfiguration Get(string environmentName)
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
