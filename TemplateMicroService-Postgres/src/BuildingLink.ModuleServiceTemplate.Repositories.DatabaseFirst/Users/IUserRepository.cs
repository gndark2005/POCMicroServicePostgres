using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsersByUnitId(int unitId);

        Task<IEnumerable<UserPermissionSetting>> GetUserPermissionSettingAsync(int userId, int permissionId);
    }
}
