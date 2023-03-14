using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using BuildingLink.Core.Diagnostics;
using BuildingLink.Core.Serialization.Json;
using BuildingLink.Messaging;
using BuildingLink.Messaging.MassTransit;
using BuildingLink.Messaging.Publisher;
using BuildingLink.ModuleServiceTemplate.Authentication;
using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Services.Books;
using BuildingLink.ModuleServiceTemplate.Services.Books.Validators;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee;
using BuildingLink.Services.Authentication;
using BuildingLink.Services.Authentication.Transformations;
using BuildingLink.Services.HealthChecks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingLink.ModuleServiceTemplate.Configuration
{
    /// <summary>
    /// Static class used to add layers of services to the ASP.NET core service collection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add presentation services (Swagger, CORS, Controllers) to the ASP.NET core service collection.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Setup());

            services.AddAutoMapper(typeof(Services.Books.Mapping.BookMapping));

            // assumes Module Service assembly is BuildingLink.xxx.dll
            var moduleName = $"{Assembly.GetExecutingAssembly().GetName().Name}".Split('.')[1];

            #region Swagger configuration

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(swaggerGenOptions =>
            {
                // Use method name as operationId
                swaggerGenOptions.CustomOperationIds(apiDescription =>
                    $"{apiDescription.ActionDescriptor.RouteValues["controller"]}{(apiDescription.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null)}");

                swaggerGenOptions.AddSecurityDefinition(
                    "oauth2",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(configuration["Authentication:Authority"] + "/connect/authorize"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "module_service_read", "Access read operations" },
                                    { "module_service_write", "Access write operations" },
                                    { "internal_website_apis", "Access read/write operations" },
                                    { "internal_staff_app_apis", "Access read/write operations" },
                                },
                            },
                        },
                    });

                swaggerGenOptions.SwaggerDoc(
                    "PropertyEmployee-v1",
                    new OpenApiInfo
                    {
                        Description = "This is the Content Creator API Module Service",
                        Title = $"{moduleName} / PropertyEmployee / 1.0",
                        Version = $"{moduleName}/PropertyEmployee/v1",
                        TermsOfService = new Uri("https://buildinglink.com"),
                        Contact = new OpenApiContact
                        {
                            Name = "BL Team",
                            Email = "admin@buildinglink.com",
                        },
                        License = new OpenApiLicense
                        {
                            Name = $"BuildingLink Inc © {DateTime.Now.Year}",
                            Url = new Uri("https://buildinglink.com"),
                        },
                    });

                swaggerGenOptions.SwaggerDoc(
                    "Resident-v1",
                    new OpenApiInfo
                    {
                        Description = "This is the Content Creator API Module Service",
                        Title = $"{moduleName} / Resident / 1.0",
                        Version = $"{moduleName}/Resident/v1",
                        TermsOfService = new Uri("https://buildinglink.com"),
                        Contact = new OpenApiContact
                        {
                            Name = "BL Team",
                            Email = "admin@buildinglink.com",
                        },
                        License = new OpenApiLicense
                        {
                            Name = $"BuildingLink Inc © {DateTime.Now.Year}",
                            Url = new Uri("https://buildinglink.com"),
                        },
                    });

                // Locate the XML file being generated by ASP.NET...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swaggerGenOptions.IncludeXmlComments(xmlPath);
            });

            #endregion

            var corsConfiguration = configuration.GetSection(Cors.CorsConfigurationKey).Get<Cors>();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: Cors.CorsPolicyName,
                    builder =>
                    {
                        builder
                            .WithOrigins(corsConfiguration.Origins.ToArray());
                        if (corsConfiguration.Methods != null)
                        {
                            builder.WithMethods(corsConfiguration.Methods.ToArray());
                        }
                        else
                        {
                            builder.AllowAnyMethod();
                        }

                        if (corsConfiguration.Headers != null)
                        {
                            builder.WithHeaders(corsConfiguration.Headers.ToArray());
                        }
                        else
                        {
                            builder.AllowAnyHeader();
                        }
                    });
            });

            return services;
        }

        /// <summary>
        /// Add business layer services.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddBusinessService(this IServiceCollection services)
        {
            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();

            return services.AddTransient<IBookStore, BookStore>();
        }

        /// <summary>
        /// Add data access services and initialize Db context.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            // In this case the "ConnectionString" is just the database's name, because we are using "UseInMemoryDatabase" feature.
            var codeFirstConnectionString = configuration.GetConnectionString("CodeFirstDb");
            var dataBaseFirstConnectionString = configuration.GetConnectionString("DatabaseFirstDb");

            services
                .AddDbContext<CodeFirstDbContext>(
                    options => options.UseSqlServer(codeFirstConnectionString));

            services
                .AddDbContext<BuildingLinkContext>(
                    options => options.UseSqlServer(dataBaseFirstConnectionString, configuration =>
                    {
                        configuration.UseHierarchyId();
                    }));

            // Data access services
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IBookSeedingService, BookSeedingService>();

            // Data access service to User
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            return services;
        }

        /// <summary>
        /// Add authentication services to the ASP.NET core service collection.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <param name="environment">Host environment.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            Action<AuthenticationFakeOptions> configureFakeOptions = null;

            var useFake = (environment.IsTesting()
                            || environment.IsLocal())
                          && bool.Parse(configuration["Authentication:Fake:IsActive"]);

            if (useFake)
            {
                configureFakeOptions = options =>
                {
                    options.UserId = configuration["Authentication:Fake:Identity:UserId"];
                    options.UserTypeNode = configuration["Authentication:Fake:Identity:UserTypeNode"];
                    options.PropertyId = configuration["Authentication:Fake:Identity:PropertyId"];
                    options.Locale = configuration["Authentication:Fake:Identity:Locale"];
                    options.OccupancyId = configuration["Authentication:Fake:Identity:OccupancyId"];
                    options.TimeZone = configuration["Authentication:Fake:Identity:TimeZone"];
                };
            }

            return services.AddAuthentication(
                options =>
                {
                    options.Authority = configuration["Authentication:Authority"];
                },
                configureFakeOptions,
                new List<UserTypeToRoleMapping>
                {
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:SecurityOfficer"],
                        new List<string> { Roles.Creator.ToString() }),
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:Management"],
                        new List<string> { Roles.Creator.ToString() }),
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:Maintenance"],
                        new List<string> { Roles.Reader.ToString() }),
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:FrontDesk"],
                        new List<string> { Roles.Reader.ToString() }),
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:Resident"],
                        new List<string> { Roles.Reader.ToString() }),
                    new UserTypeToRoleMapping(
                        configuration["Authentication:AllowedUserTypes:PublicDisplay"],
                        new List<string> { Roles.Reader.ToString() })
                });
        }

        /// <summary>
        /// Add healthy services to the ASP.NET core service collection.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <param name="environment">Host environment.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddHealthServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            services
                .AddSingleton<IResourceUsage, OsMemoryUsage>()
                .Configure<ResourceUsageHealthCheckOptions>(nameof(MemoryUsageHealthCheck), options =>
                {
                    options.ResourceName = MemoryUsageHealthCheck.Name;
                    options.LowUsageThreshold = int.Parse(configuration["HealthChecks:MemoryUsage:LowUsageThreshold"]);
                    options.HighUsageThreshold =
                        int.Parse(configuration["HealthChecks:MemoryUsage:HighUsageThreshold"]);
                })
                .AddSingleton(provider => new MemoryUsageHealthCheck(
                    provider.GetService<IOptionsMonitor<ResourceUsageHealthCheckOptions>>(),
                    provider.GetService<IEnumerable<IResourceUsage>>().OfType<OsMemoryUsage>().Single(),
                    provider.GetService<ILogger<ResourceUsageHealthCheck>>()))
                .AddSingleton<IResourceUsage, ProcessCpuUsage>()
                .Configure<ResourceUsageHealthCheckOptions>(nameof(CpuUsageHealthCheck), options =>
                {
                    options.ResourceName = CpuUsageHealthCheck.Name;
                    options.LowUsageThreshold = int.Parse(configuration["HealthChecks:CpuUsage:LowUsageThreshold"]);
                    options.HighUsageThreshold = int.Parse(configuration["HealthChecks:CpuUsage:HighUsageThreshold"]);
                })
                .AddSingleton(provider => new CpuUsageHealthCheck(
                    provider.GetService<IOptionsMonitor<ResourceUsageHealthCheckOptions>>(),
                    provider.GetService<IEnumerable<IResourceUsage>>().OfType<ProcessCpuUsage>().Single(),
                    provider.GetService<ILogger<ResourceUsageHealthCheck>>()))
                .AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("CodeFirstDb"), tags: new[] { "database" })
                .AddCheck<MemoryUsageHealthCheck>(nameof(MemoryUsageHealthCheck), tags: new[] { MemoryUsageHealthCheck.Tag })
                .AddCheck<CpuUsageHealthCheck>(nameof(CpuUsageHealthCheck), tags: new[] { CpuUsageHealthCheck.Tag });

            return services;
        }

        /// <summary>
        /// Add BuildingLink Messaging configuration to service collention.
        /// </summary>
        /// <param name="services">ASP.NET core service collection.</param>
        /// <param name="environment">Host environment.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>ASP.NET core service collection.</returns>
        public static IServiceCollection AddMessaging(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            var messagingSettings = configuration.GetSection(MessagingSettings.SectionName).Get<MessagingSettings>();
            if (messagingSettings == null)
            {
                throw new ArgumentNullException(nameof(MessagingSettings));
            }

            var useFake = (environment.IsLocal() || environment.IsTesting())
                && messagingSettings.Fake.IsActive;

            if (useFake)
            {
                return services.AddMessagingFakeServices();
            }

            return services
                .AddPublisherService()
                .AddMessagingServices(brokerConfiguration =>
                {
                    brokerConfiguration.Host = messagingSettings.Broker.Host;
                    brokerConfiguration.VirtualHost = messagingSettings.Broker.VirtualHost;
                    brokerConfiguration.Username = messagingSettings.Broker.Username;
                    brokerConfiguration.Password = messagingSettings.Broker.Password;
                });
        }
    }
}