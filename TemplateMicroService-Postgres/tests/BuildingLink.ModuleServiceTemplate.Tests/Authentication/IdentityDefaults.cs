using System.Diagnostics.CodeAnalysis;
using BuildingLink.Services.Authentication;

namespace BuildingLink.ModuleServiceTemplate.Tests.Authentication
{
    [ExcludeFromCodeCoverage]
    public class IdentityDefaults
    {
        public const int UserId = 1;

        public const string UserTypeNode = UserTypes.PropertyEmployee.Management.HierarchyNode;

        public const int PropertyId = 1;

        public const string Locale = "en";

        public const int OccupancyId = 100;

        public const string TimeZone = "America/New_York";

        public const string TimeZoneLA = "America/Los_Angeles";

        public const string TimeZoneUY = "America/Montevideo";
    }
}
