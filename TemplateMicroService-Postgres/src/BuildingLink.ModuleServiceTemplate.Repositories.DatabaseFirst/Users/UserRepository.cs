using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly BuildingLinkContext _buildingLinkContext;

        public UserRepository(BuildingLinkContext buildinglinkContext)
        {
            _buildingLinkContext = buildinglinkContext;
        }

        public IQueryable<User> GetUsersByUnitId(int unitId)
        {
            return _buildingLinkContext.Users
                .Where(u => u.UnitId == unitId)
                .Include(u => u.Unit);
        }

        public async Task<IEnumerable<UserPermissionSetting>> GetUserPermissionSettingAsync(int userId, int permissionId)
        {
            return await _buildingLinkContext.Procedures.GetUserPermissionSettingAsync(userId, permissionId);
        }
    }
}
