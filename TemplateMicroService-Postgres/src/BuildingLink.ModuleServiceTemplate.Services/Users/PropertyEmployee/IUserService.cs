using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;

namespace BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee
{
    public interface IUserService
    {
        IEnumerable<UserResponseDto> GetUsersByUnitId(int unitId);

        Task<IEnumerable<UserPermissionSetting>> GetCurrentUserPermissionSettingAsync(int permissionId);
    }
}
