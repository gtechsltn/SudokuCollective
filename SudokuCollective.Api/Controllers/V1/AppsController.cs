using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// App Controller Class
    /// </summary>
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        /// <summary>
        /// App get method
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get result from app controller...");
        }
    }
}
