using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace BuildingLink.ModuleServiceTemplate.Configuration
{
    /// <summary>
    /// Environment helpers.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Environments
    {
        /// <summary>
        /// End to end test environment.
        /// </summary>
        public const string EndToEnd = "EndToEnd";

        /// <summary>
        /// Get current environment.
        /// </summary>
        /// <returns>Environment name.</returns>
        public static string Get()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return !string.IsNullOrEmpty(environmentName)
                ? environmentName
                : "Local";
        }

        public static bool IsLocal(this IHostEnvironment environment)
            => string.Equals(environment.EnvironmentName, "Local", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsTesting(this IHostEnvironment environment)
            => string.Equals(environment.EnvironmentName, "Testing", StringComparison.InvariantCultureIgnoreCase);
    }
}
