using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using AutoMapper;
using BuildingLink.Core.Serialization.Json;
using BuildingLink.ModuleServiceTemplate.Configuration;
using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using BuildingLink.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingLink.ModuleServiceTemplate.Tests.Controllers
{
    public abstract class IntegrationTestsCodeFirstBase : WebApplicationFactory<Startup>
    {
        protected IntegrationTestsCodeFirstBase()
        {
            Client = Server.CreateClient();

            var scope = Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            DbContext = scope.ServiceProvider.GetRequiredService<CodeFirstDbContext>();
            DbContext.Database.EnsureCreated();

            JsonSerializerOptions = new JsonSerializerOptions().Setup();

            Mapper = Server.Services.GetService<IMapper>();
        }

        public HttpClient Client { get; }

        public CodeFirstDbContext DbContext { get; }

        public JsonSerializerOptions JsonSerializerOptions { get; }

        public IMapper Mapper { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var environment = Environments.Get();
            var configuration = ConfigurationManager.Get(environment);

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
                        typeof(DbContextOptions<CodeFirstDbContext>))
                    .AddTestCodeFirstDbContextServices());
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
