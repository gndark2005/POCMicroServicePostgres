using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using AutoMapper;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;
using BuildingLink.Services.Authentication;

namespace BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee
{
    public class UserService : IUserService
    {
        private readonly IIdentityContext _identityContext;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper, IIdentityContext identityContext)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _identityContext = identityContext;
        }

        public IEnumerable<UserResponseDto> GetUsersByUnitId(int unitId)
        {
            if (!_identityContext.IsAuthenticated)
            {
                throw new AuthenticationException("An Authenticated user is needed.");
            }

            return _userRepository.GetUsersByUnitId(unitId).Where(u => u.IsActive)
                .Select(user => _mapper.Map<UserResponseDto>(user))
                .ToList();
        }

        public Task<IEnumerable<UserPermissionSetting>> GetCurrentUserPermissionSettingAsync(int permissionId)
        {
            if (!_identityContext.IsAuthenticated)
            {
                throw new AuthenticationException("An Authenticated user is needed.");
            }

            return _userRepository.GetUserPermissionSettingAsync(_identityContext.UserId.Value, permissionId);
        }
    }
}
