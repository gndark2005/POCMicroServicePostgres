using System.Diagnostics.CodeAnalysis;
using BuildingLink.Core.Diagnostics;
using BuildingLink.Messaging.MassTransit;
using BuildingLink.ModuleServiceTemplate.Configuration;
using BuildingLink.ModuleServiceTemplate.Consumer.Consumers;
using BuildingLink.ModuleServiceTemplate.Consumers;
using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Services.Books;
using BuildingLink.Services.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BuildingLink.ModuleServiceTemplate.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            var codeFirstConnectionString = configuration.GetConnectionString("CodeFirstDb");

            services
                .AddDbContext<CodeFirstDbContext>(
                    options => options.UseSqlServer(codeFirstConnectionString));

            // Data access services
            services.AddTransient<IBookRepository, BookRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessService(this IServiceCollection services)
        {
            return services.AddTransient<BookStore>();
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var messagingSettings = configuration.GetSection(MessagingSettings.SectionName).Get<MessagingSettings>();

            if (messagingSettings == null)
            {
                throw new ArgumentNullException(nameof(MessagingSettings));
            }

            return services.AddMessagingServices(
                rabbitMqConfiguration =>
                {
                    rabbitMqConfiguration.Host = messagingSettings.Broker.Host;
                    rabbitMqConfiguration.VirtualHost = messagingSettings.Broker.VirtualHost;
                    rabbitMqConfiguration.Username = messagingSettings.Broker.Username;
                    rabbitMqConfiguration.Password = messagingSettings.Broker.Password;
                },
                queuesConfigurator =>
                {
                    queuesConfigurator.ForQueue(nameof(BookCreated))
                        .AddConsumer<BookCreatedConsumer>()
                        .WithMessageRetryDeliveryCount(messagingSettings.RetryDeliveryCount)
                        .WithMessageRetryIntervalMs(messagingSettings.RetryDeliveryIntervalMs);
                    queuesConfigurator.ForQueue(nameof(BookStatusChanged))
                        .AddConsumer<BookStatusChangedConsumer>()
                        .WithMessageRetryDeliveryCount(messagingSettings.RetryDeliveryCount)
                        .WithMessageRetryIntervalMs(messagingSettings.RetryDeliveryIntervalMs);
                    queuesConfigurator.ForQueue(nameof(BookRemoved))
                        .AddConsumer<BookRemovedConsumer>()
                        .WithMessageRetryDeliveryCount(messagingSettings.RetryDeliveryCount)
                        .WithMessageRetryIntervalMs(messagingSettings.RetryDeliveryIntervalMs);
                });
        }

        public static IServiceCollection AddHealthServices(this IServiceCollection services, IConfiguration configuration)
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
                    provider.GetService<ILogger<ResourceUsageHealthCheck>>()));

            services.AddHealthChecks()
                .AddCheck<MemoryUsageHealthCheck>(nameof(MemoryUsageHealthCheck), tags: new[] { MemoryUsageHealthCheck.Tag })
                .AddCheck<CpuUsageHealthCheck>(nameof(CpuUsageHealthCheck), tags: new[] { CpuUsageHealthCheck.Tag });

            return services;
        }
    }
}