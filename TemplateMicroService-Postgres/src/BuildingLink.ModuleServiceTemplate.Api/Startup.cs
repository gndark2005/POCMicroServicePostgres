using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BuildingLink.Middlewares.ErrorHandler;
using BuildingLink.ModuleServiceTemplate.Api.Logging;
using BuildingLink.ModuleServiceTemplate.Configuration;
using BuildingLink.Services.HealthChecks;
using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using BuildingLinkServiceCollectionExtensions = BuildingLink.ModuleServiceTemplate.Configuration.ServiceCollectionExtensions;

namespace BuildingLink.ModuleServiceTemplate
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient(_environment.ApplicationName)
               .AddCorrelationIdForwarding();
            services.AddCorrelationId(options => options.IncludeInResponse = true).WithTraceIdentifierProvider();

            services.AddTransient<IdentityEnricher>();
            BuildingLinkServiceCollectionExtensions.AddHealthServices(services, _environment, _configuration);
            services.AddAuthenticationServices(_environment, _configuration);
            services.AddPresentationServices(_configuration);
            services.AddBusinessService();
            services.AddDataServices(_configuration);
            services.AddMessaging(_environment, _configuration);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable custom middleware to handler API errors.
            app.UseExceptionHandler(options => options.UseApiErrorHandlerMiddleware(env));

            #region Swagger documentation

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(configuration =>
            {
                configuration.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"https://{httpReq.Host.Value}", Description = "Localhost" },
                        new OpenApiServer { Url = $"{_configuration["ApiGateway:BaseUri"]}/{swagger.Info.Version}", Description = "API Gateway" }
                    };
                });
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/PropertyEmployee-v1/swagger.json", "ModuleServiceTemplate-PropertyEmployee-v1");
                c.SwaggerEndpoint("/swagger/Resident-v1/swagger.json", "ModuleServiceTemplate-Resident-v1");
                c.RoutePrefix = string.Empty;
            });

            #endregion

            app.UseHttpsRedirection()
                .UseCorrelationId()
                .UseRouting()
                .UseCors(Cors.CorsPolicyName)
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.RegisterHealthProbesMapping();

                    endpoints.MapControllers();
                });
        }
    }
}
