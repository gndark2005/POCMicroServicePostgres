using System.Diagnostics.CodeAnalysis;
using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingLink.ModuleServiceTemplate
{
    /// <summary>
    /// Provide extensions method for the Host proccess.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class HostExtensions
    {
        /// <summary>
        /// Initialize the database with a data collection.
        /// </summary>
        /// <param name="host">Host proccess.</param>
        /// <returns>Host proccess.</returns>
        public static IHost SeedData(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var bookSeedingService = services.GetService<IBookSeedingService>();

            bookSeedingService.SeedBooks();

            return host;
        }

        /// <summary>
        /// Remove database and migrate after remove.
        /// </summary>
        /// <param name="host">Host proccess.</param>
        /// <returns>Host proccess.</returns>
        public static IHost CleanData(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var moduleServiceTemplateDbContext = services.GetService<CodeFirstDbContext>();

            if (moduleServiceTemplateDbContext != null)
            {
                moduleServiceTemplateDbContext.Database.EnsureDeleted();
                Log.Information("Database is deleted");

                moduleServiceTemplateDbContext.Database.Migrate();
                Log.Information("Database Migration is finished");
            }
            else
            {
                Log.Error("Inspections Db Context was null during CleanData");
            }

            return host;
        }
    }
}
