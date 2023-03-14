using System;
using System.Net.Http;
using System.Text.Json;
using AutoMapper;
using BuildingLink.Core.Serialization.Json;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using BuildingLink.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ConfigurationManager = BuildingLink.ModuleServiceTemplate.Configuration.ConfigurationManager;

namespace BuildingLink.ModuleServiceTemplate.Tests.Controllers
{
    public abstract class IntegrationTestsDatabaseFirstBase : WebApplicationFactory<Startup>
    {
        protected IntegrationTestsDatabaseFirstBase()
        {
            Client = Server.CreateClient();

            var scope = Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            Configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            DbContext = scope.ServiceProvider.GetRequiredService<BuildingLinkContext>();
            DbContext.Database.EnsureCreated();

            DbContext.InitializeStoredProcedures();

            JsonSerializerOptions = new JsonSerializerOptions().Setup();

            Mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        }

        public IConfiguration Configuration { get; }

        public HttpClient Client { get; }

        public BuildingLinkContext DbContext { get; }

        public JsonSerializerOptions JsonSerializerOptions { get; }

        public IMapper Mapper { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = ConfigurationManager.Get(environment ?? "Local");

            var connectionString = configuration.GetConnectionString("DatabaseFirstDb");
            var masterConnectionString = connectionString.Replace("Initial Catalog=BuildingLink;", string.Empty);
            var databaseName = $"BuildingLink_{Guid.NewGuid()}";
            connectionString = $"{masterConnectionString};Initial Catalog={databaseName};";
            builder.ConfigureTestServices(services =>
                services
                    .RemoveServices(
                        typeof(AuthenticationFakeHandler),
                        typeof(IConfigureOptions<AuthenticationOptions>),
                        typeof(IConfigureOptions<AuthenticationFakeOptions>),
                        typeof(IValidateOptions<AuthenticationFakeOptions>))
                    .AddAuthenticationFakeServices()
                    .RemoveServices(
                        typeof(DbContextOptions),
                        typeof(DbContextOptions<BuildingLinkContext>))
                    .AddTestDbFirstContextServices(connectionString));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (DbContext != null)
            {
                DbContext.Database.EnsureDeleted();
                DbContext.Dispose();
            }

            Client?.Dispose();
        }
    }
}
