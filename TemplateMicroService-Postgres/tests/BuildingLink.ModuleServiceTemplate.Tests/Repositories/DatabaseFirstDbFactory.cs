using System;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = BuildingLink.ModuleServiceTemplate.Tests.Configuration.ConfigurationManager;

namespace BuildingLink.ModuleServiceTemplate.Tests.Repositories
{
    public class DatabaseFirstDbFactory : IDisposable
    {
        private readonly string masterConnectionString;
        private readonly string databaseName;

        public DatabaseFirstDbFactory()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = ConfigurationManager.Get(environment ?? "Local");
            var connectionString = configuration.GetConnectionString("DatabaseFirstDb");

            masterConnectionString = connectionString.Replace("Initial Catalog=BuildingLink;", string.Empty);
            databaseName = $"BuildingLink_{Guid.NewGuid()}";
            connectionString = $"{masterConnectionString};Initial Catalog={databaseName};";

            BuildingLinkContext = new BuildingLinkContext(
                new DbContextOptionsBuilder<BuildingLinkContext>()
                    .UseSqlServer(connectionString, configuration =>
                    {
                        configuration.UseHierarchyId();
                    })
                    .Options);

            BuildingLinkContext.Database.EnsureCreated();

            BuildingLinkContext.InitializeStoredProcedures();

            UserTypeNodeFakeConfiguration = configuration.GetValue<string>("Authentication:Fake:Identity:UserTypeNode");
        }

        protected BuildingLinkContext BuildingLinkContext { get; private set; }

        protected string UserTypeNodeFakeConfiguration { get; private set; }

        public void Dispose()
        {
            BuildingLinkContext.Database.EnsureDeleted();
            BuildingLinkContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
