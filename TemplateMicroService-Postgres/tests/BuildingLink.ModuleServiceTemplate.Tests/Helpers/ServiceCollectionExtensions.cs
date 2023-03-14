using System;
using System.Linq;
using BuildingLink.ModuleServiceTemplate.Authentication;
using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using BuildingLink.ModuleServiceTemplate.Tests.Authentication;
using BuildingLink.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingLink.ModuleServiceTemplate.Tests.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RemoveServices(this IServiceCollection services, params Type[] serviceTypes)
        {
            foreach (var descriptor in services
                .Where(descriptor => serviceTypes
                    .Any(serviceType => descriptor.ServiceType == serviceType))
                .ToList())
            {
                services.Remove(descriptor);
            }

            return services;
        }

        public static IServiceCollection AddAuthenticationFakeServices(
            this IServiceCollection services,
            string userTypeNode = IdentityDefaults.UserTypeNode)
        {
            return services.AddAuthenticationFake(options =>
            {
                options.UserId = IdentityDefaults.UserId.ToString();
                options.UserTypeNode = userTypeNode;
                options.PropertyId = IdentityDefaults.PropertyId.ToString();
                options.Locale = IdentityDefaults.Locale;
                options.OccupancyId = IdentityDefaults.OccupancyId.ToString();
                options.TimeZone = IdentityDefaults.TimeZone;

                if (userTypeNode == UserTypes.PropertyEmployee.Management.HierarchyNode || userTypeNode == UserTypes.PropertyEmployee.FrontDesk.HierarchyNode)
                {
                    options.Roles = new[] { Roles.Reader.ToString() };
                }
                else if (userTypeNode == UserTypes.PropertyEmployee.Management.HierarchyNode)
                {
                    options.Roles = new[] { Roles.Creator.ToString() };
                }
            });
        }

        public static IServiceCollection AddTestCodeFirstDbContextServices(this IServiceCollection services)
        {
            return services
                .AddDbContext<CodeFirstDbContext>(
                    options => options.UseInMemoryDatabase("CodeFirstDb"));
        }

        public static IServiceCollection AddTestDbFirstContextServices(this IServiceCollection services, string connectionString)
        {
            return services
                .AddDbContext<BuildingLinkContext>(
                    options => options.UseSqlServer(connectionString, configuration =>
                    {
                        configuration.UseHierarchyId();
                    }));
        }
    }
}
