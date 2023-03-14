using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BuildingLink.ModuleServiceTemplate.Controllers
{
    /// <summary>
    /// Hello word controller example.
    /// </summary>
    [ApiController]
    [Route("helloword")]
    public class HelloWordController : ControllerBase
    {
        /// <summary>Hello word endpoint example.</summary>
        /// <returns>Returns a "Hello Word".</returns>
        /// <response code ="200">Returns a "Hello Word".</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Hello Word.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpGet]
        public IActionResult HelloWord()
        {
            return Ok("Hello word.");
        }
    }
}
