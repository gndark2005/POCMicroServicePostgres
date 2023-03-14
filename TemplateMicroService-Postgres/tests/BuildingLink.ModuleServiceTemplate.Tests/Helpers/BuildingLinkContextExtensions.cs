using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.SharedEntities;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Tests.Helpers
{
    public static class BuildingLinkContextExtensions
    {
        public static void InitializeSharedEntities(this BuildingLinkContext buildingLinkContext, out Facility facility, out Unit unit)
        {
            facility = new Facility()
            {
                Name = "Integration Test Building 4",
                ManagingAgentsId = 4,
                CRMBuildingTypeId = 5,
            };
            buildingLinkContext.Facilities.Add(facility);
            buildingLinkContext.SaveChanges();

            unit = new Unit()
            {
                ApartmentLabel = "Apto.",
                ApartmentId = "2A",
                AlternativeName = "Jhon",
                CompanyOrFamilyName = "Jhonson",
                CreatedAt = null,
                DeactivationDate = null,
                FacilityId = facility.Id,
                PhysicalUnitId = 1256,
                IsActive = true
            };
            buildingLinkContext.Units.Add(unit);

            buildingLinkContext.SaveChanges();
        }

        public static UserSubType InitializeUserSubType(this BuildingLinkContext buildingLinkContext, string userTypeNode)
        {
            var userSubType = new UserSubType()
            {
                Id = 1,
                Node = HierarchyId.Parse(userTypeNode),
            };

            buildingLinkContext.UserSubTypes.Add(userSubType);
            buildingLinkContext.SaveChanges();

            return userSubType;
        }

        public static void InitializeStoredProcedures(this BuildingLinkContext buildinglinkContext)
        {
            var storedProcedureFiles = Directory
                .EnumerateFiles(Path.Combine(GetCurrentAssemblyPath(), "SQL", "StoredProcedures"), "*.*", SearchOption.AllDirectories)
                .Where(file => Path.GetExtension(file).TrimStart('.').ToLowerInvariant() == "sql");
            foreach (var storedProcedureFile in storedProcedureFiles)
            {
                string sqlContent = File.ReadAllText(storedProcedureFile);

                buildinglinkContext.Database.ExecuteSqlRaw(sqlContent);
            }
        }

        public static User InitializeUser(this BuildingLinkContext buildingLinkContext, Facility facility, Unit unit, UserSubType userSubType)
        {
            var user = new User()
            {
                FacilityId = facility.Id,
                UnitId = unit.Id,
                TypeId = 2,
                FirstName = "ABC",
                MiddleName = "BCD",
                LastName = "XYZ",
                UserName = "ABCBCDXYZ",
                IsActive = true,
                SubTypeId = userSubType.Id,
            };

            buildingLinkContext.Users.Add(user);
            buildingLinkContext.SaveChanges();

            return user;
        }

        private static string GetCurrentAssemblyPath()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirOutPutPath = Path.GetDirectoryName(codeBasePath);
            return dirOutPutPath;
        }
    }
}
