using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BuildingLink.ModuleServiceTemplate.Configuration
{
    /// <summary>
    /// Configurations helper.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ConfigurationManager
    {
        /// <summary>
        /// Load configuration.
        /// </summary>
        /// <param name="environmentName">Environment name.</param>
        /// <returns>Configuration.</returns>
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
