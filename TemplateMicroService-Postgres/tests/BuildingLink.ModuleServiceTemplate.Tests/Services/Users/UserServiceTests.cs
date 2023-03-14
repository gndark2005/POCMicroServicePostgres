using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using AutoMapper;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.Mapping;
using BuildingLink.Services.Authentication;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Services.Users
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Services")]
    [Trait("Category", "Unit")]
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IIdentityContext> _identityContextMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserMapping());
            });
            _mapper = mapperConfiguration.CreateMapper();

            _userRepositoryMock = new Mock<IUserRepository>();
            _identityContextMock = new Mock<IIdentityContext>();

            _userService = new UserService(_userRepositoryMock.Object, _mapper, _identityContextMock.Object);
        }

        [Fact]
        public void GetUsersByUnitId_ShouldThrowAnExceptionIfTheUserIsNotAuthenticated()
        {
            // ARRANGE
            _identityContextMock.SetupGet(identityContext => identityContext.IsAuthenticated).Returns(false);

            // ASSERT
            Assert.Throws<AuthenticationException>(() => _userService.GetUsersByUnitId(default(int)));
        }

        [Fact]
        public void GetUsersByUnitId_ReturnsActiveUsersByUnit()
        {
            // ARRANGE
            var unitId = 40;
            var expectedUsers = new User[]
            {
                new User()
                {
                    FacilityId = 4,
                    UnitId = unitId,
                    TypeId = 2,
                    FirstName = "ABC",
                    MiddleName = "BCD",
                    LastName = "XYZ",
                    UserName = "ABCBCDXYZ",
                    IsActive = true,
                    SubTypeId = 2,
                },
                new User()
                {
                    FacilityId = 4,
                    UnitId = unitId,
                    TypeId = 2,
                    FirstName = "ABC",
                    MiddleName = "BCD",
                    LastName = "XYZ",
                    UserName = "ABCBCDXYZ",
                    IsActive = true,
                    SubTypeId = 3,
                },
            };

            _identityContextMock.SetupGet(identityContext => identityContext.IsAuthenticated).Returns(true);
            _userRepositoryMock.Setup(userRepository => userRepository.GetUsersByUnitId(unitId))
                .Returns(expectedUsers.AsQueryable());

            // ACT
            var users = _userService.GetUsersByUnitId(unitId);

            // ASSERT
            users.Should().BeEquivalentTo(users.Select(user => _mapper.Map<UserResponseDto>(user)));
        }

        [Fact]
        public void GetUsersByUnitId_DoNotReturnsNonActiveUser()
        {
            // ARRANGE
            var unitId = 40;
            var expectedUsers = new User[]
            {
                new User()
                {
                    FacilityId = 4,
                    UnitId = unitId,
                    TypeId = 2,
                    FirstName = "ABC",
                    MiddleName = "BCD",
                    LastName = "XYZ",
                    UserName = "ABCBCDXYZ",
                    IsActive = false,
                    SubTypeId = 2,
                },
            };

            _identityContextMock.SetupGet(identityContext => identityContext.IsAuthenticated).Returns(true);
            _userRepositoryMock.Setup(userRepository => userRepository.GetUsersByUnitId(unitId))
                .Returns(expectedUsers.AsQueryable());

            // ACT
            var users = _userService.GetUsersByUnitId(unitId);

            // ASSERT
            users.Should().BeEquivalentTo(Array.Empty<UserResponseDto>());
        }

        [Fact]
        public async Task GetCurrentUserPermissionSettingAsync_ShouldThrowAnExceptionIfTheUserIsNotAuthenticated()
        {
            // ARRANGE
            _identityContextMock.SetupGet(identityContext => identityContext.IsAuthenticated).Returns(false);

            // ASSERT
            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.GetCurrentUserPermissionSettingAsync(default(int)));
        }

        [Fact]
        public async Task GetCurrentUserPermissionSettingAsync_ReturnsuserPermissionSettingsRetunedbyTheRepository()
        {
            // ARRANGE
            var userId = 24;
            var permissionId = 3;
            var expectedUserPermissionSetting = new UserPermissionSetting[]
            {
                new UserPermissionSetting()
                {
                    PermissionId = 2,
                    BuildingTypeId = 3,
                    ManagingAgencyId = 54,
                    BuildingId = 4,
                    UserTypeId = 3,
                    PhysicalUnitId = 41542,
                    OccupancyId = 40,
                    UserId = 24,
                    SubTypeId = 5,
                    OptionId = 5,
                    CreateDateUTC = DateTime.UtcNow,
                    CreateUserId = 54,
                    ChangeDateUTC = DateTime.UtcNow,
                    ChangeUserId = 25,
                }
            };

            _identityContextMock.SetupGet(identityContext => identityContext.IsAuthenticated).Returns(true);
            _identityContextMock.SetupGet(identityContext => identityContext.UserId).Returns(userId);
            _userRepositoryMock.Setup(userRepository => userRepository.GetUserPermissionSettingAsync(userId, permissionId))
                .ReturnsAsync(expectedUserPermissionSetting);

            // ACT
            var users = await _userService.GetCurrentUserPermissionSettingAsync(permissionId);

            // ASSERT
            users.Should().BeEquivalentTo(expectedUserPermissionSetting);
        }
    }
}
