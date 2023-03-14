using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.SharedEntities;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Repositories
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Repositories")]
    [Trait("Category", "Unit")]
    public class UserRepositoryTests : DatabaseFirstDbFactory
    {
        private readonly UserSubType _userSubType;

        public UserRepositoryTests()
            : base()
        {
            _userSubType = BuildingLinkContext.InitializeUserSubType(UserTypeNodeFakeConfiguration);
        }

        [Fact]
        public async Task GetAll_ReturnUsersByUnit()
        {
            // ARRANGE
            BuildingLinkContext.InitializeSharedEntities(out Facility facility, out Unit unit);
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
                    SubTypeId = _userSubType.Id,
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
                    IsActive = true,
                    SubTypeId = _userSubType.Id,
                }
            };

            await BuildingLinkContext.Users.AddRangeAsync(expectedUsers);
            await BuildingLinkContext.SaveChangesAsync();

            // ACT
            var userRepository = new UserRepository(BuildingLinkContext);
            var users = await userRepository.GetUsersByUnitId(unit.Id)
                .ToListAsync();

            // ASSERT
            expectedUsers.Should().Equal(users);
        }

        [Fact]
        public async Task GetUserPermissionSettingAsync_ReturnPermissionSettingByUserAndPermission()
        {
            // ARRANGE
            BuildingLinkContext.InitializeSharedEntities(out Facility facility, out Unit unit);
            var user = BuildingLinkContext.InitializeUser(facility, unit, _userSubType);
            var createAndChangeUser = BuildingLinkContext.InitializeUser(facility, unit, _userSubType);
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
                SubTypeId = _userSubType.Id,
                OptionId = 5,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = createAndChangeUser.Id,
                ChangedDate = DateTime.UtcNow,
                ChangeUserId = createAndChangeUser.Id,
            };

            BuildingLinkContext.PermissionSettings.Add(permissionSetting);
            BuildingLinkContext.SaveChanges();

            BuildingLinkContext.Entry(permissionSetting).Reload();

            // ACT
            var userRepository = new UserRepository(BuildingLinkContext);
            var userPermissionSetting = await userRepository.GetUserPermissionSettingAsync(user.Id, permissionSetting.PermissionId);

            // ASSERT
            userPermissionSetting.Should().BeEquivalentTo(new UserPermissionSetting[]
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
                    SubTypeId = _userSubType.Id,
                },
            });
        }
    }
}
