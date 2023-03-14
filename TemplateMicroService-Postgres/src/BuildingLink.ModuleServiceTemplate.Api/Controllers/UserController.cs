using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Authentication;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;
using BuildingLink.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingLink.ModuleServiceTemplate.Controllers
{
    /// <summary>
    /// Hello word controller example.
    /// </summary>
    [ApiController]
    [Route("user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>User service endpoint example.</summary>
        /// <param name="unitId">Unit Id.</param>
        /// <returns>Returns "users".</returns>
        /// <response code ="200">Returns a Users by UnitId.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="500">Returns a server error.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpGet("unit/{unitId}")]
        [AuthorizedRoles(typeof(Roles), Roles.Reader, Roles.Creator)]
        public IActionResult GetUsersByUnitId(int unitId)
        {
            return Ok(_userService.GetUsersByUnitId(unitId));
        }

        /// <summary>User service endpoint example.</summary>
        /// <param name="permissionId">Permission Id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <response code ="200">Returns permission settings for current user.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="500">Returns a server error.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UserPermissionSetting>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpGet("permission/{permissionId}")]
        [AuthorizedRoles(typeof(Roles), Roles.Reader, Roles.Creator)]
        public async Task<IActionResult> GetUserPermitionSettings(int permissionId)
        {
            return Ok(await _userService.GetCurrentUserPermissionSettingAsync(permissionId));
        }
    }
}
