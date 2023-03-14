using BuildingLink.ModuleServiceTemplate.Configurations;
using BuildingLink.ModuleServiceTemplate.Consumer.Configurations;
using BuildingLink.Services.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace BuildingLink.ModuleServiceTemplate.Consumer
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
            services.AddMessaging(_configuration);
            services.AddDataServices(_configuration);
            services.AddBusinessService();
            services.AddHealthServices(_configuration);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.RegisterHealthProbesMapping();
            });
        }
    }
}
