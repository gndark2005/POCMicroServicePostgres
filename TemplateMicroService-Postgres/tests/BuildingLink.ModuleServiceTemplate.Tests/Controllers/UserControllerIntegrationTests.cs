using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.SharedEntities;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Controllers
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Controllers")]
    [Trait("Category", "Integration")]
    public class UserControllerIntegrationTests : IntegrationTestsDatabaseFirstBase
    {
        private readonly string _userTypeNodeFakeConfiguration = "/4/1/6/";

        [Fact]
        public async Task GetUsersByUnitId_Users()
        {
            // ARRANGE
            DbContext.InitializeSharedEntities(out Facility facility, out Unit unit);
            var userSubType = DbContext.InitializeUserSubType(_userTypeNodeFakeConfiguration);
            var expectedUsers = new User[]
            {
                new User()
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
                },
                new User()
                {
                    FacilityId = facility.Id,
                    UnitId = unit.Id,
                    TypeId = 1,
                    FirstName = "BCD",
                    MiddleName = "ABC",
                    LastName = "XYZ",
                    UserName = "BCDABCXYZ",
                    IsActive = false,
                    SubTypeId = userSubType.Id,
                },
            };

            await DbContext.Users.AddRangeAsync(expectedUsers);
            await DbContext.SaveChangesAsync();

            // ACT
            var response = await Client.GetAsync($"user/unit/{unit.Id}");

            // ASSERT
            response.IsSuccessStatusCode.Should().BeTrue();

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<UserResponseDto>>(jsonResult, JsonSerializerOptions);
            result.Should().BeEquivalentTo(expectedUsers.Where(user => user.IsActive).Select(user => Mapper.Map<UserResponseDto>(user)));
        }

        [Fact]
        public async Task GetCurrentUserPermissionSettingAsync_PermissionSettings()
        {
            // ARRANGE
            // ARRANGE
            DbContext.InitializeSharedEntities(out Facility facility, out Unit unit);
            var userSubType = DbContext.InitializeUserSubType(_userTypeNodeFakeConfiguration);
            var user = DbContext.InitializeUser(facility, unit, userSubType);
            var createAndChangeUser = DbContext.InitializeUser(facility, unit, userSubType);
            var permissionSetting = new PermissionSetting()
            {
                PermissionId = 2,
                BuildingTypeId = facility.CRMBuildingTypeId.Value,
                ManagingAgencyId = facility.ManagingAgentsId.Value,
                BuildingId = facility.Id,
                UserTypeId = user.TypeId,
                PhysicalUnitId = unit.PhysicalUnitId,
                OccupancyId = unit.Id,
                UserId = user.Id,
                SubTypeId = userSubType.Id,
                OptionId = 5,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = createAndChangeUser.Id,
                ChangedDate = DateTime.UtcNow,
                ChangeUserId = createAndChangeUser.Id,
            };

            DbContext.PermissionSettings.Add(permissionSetting);
            DbContext.SaveChanges();

            DbContext.Entry(permissionSetting).Reload();

            // ACT
            var response = await Client.GetAsync($"user/permission/{permissionSetting.PermissionId}");

            // ASSERT
            response.IsSuccessStatusCode.Should().BeTrue();

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<UserPermissionSetting>>(jsonResult, JsonSerializerOptions);
            result.Should().BeEquivalentTo(new UserPermissionSetting[]
            {
                new UserPermissionSetting()
                {
                    PermissionId = permissionSetting.PermissionId,
                    BuildingTypeId = permissionSetting.BuildingTypeId,
                    ManagingAgencyId = permissionSetting.ManagingAgencyId,
                    BuildingId = facility.Id,
                    UserTypeId = user.TypeId,
                    PhysicalUnitId = permissionSetting.PhysicalUnitId,
                    OccupancyId = unit.Id,
                    UserId = user.Id,
                    OptionId = permissionSetting.OptionId,
                    CreateDateUTC = permissionSetting.CreatedDate,
                    ChangeDateUTC = permissionSetting.ChangedDate,
                    CreateUserId = permissionSetting.CreatedUserId,
                    ChangeUserId = permissionSetting.ChangeUserId,
                    SubTypeId = userSubType.Id,
                },
            });
        }
    }
}
