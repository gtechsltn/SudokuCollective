using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SudokuCollective.Api.Controllers
{
    /// <summary>
    /// Index Controller Class
    /// </summary>
    [AllowAnonymous]
    [Route("api/index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private IWebHostEnvironment _environment;
        private IConfiguration _configuration { get; }

        /// <summary>
        /// IndexController constructor
        /// </summary>
        public IndexController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        
        /// <summary>
        /// A method to obtain and populate the Sudoku Collective mission statement on the index home page.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var missionStatement = !_environment.IsStaging() ? 
                _configuration.GetSection("MissionStatement").Value : 
                Environment.GetEnvironmentVariable("MISSIONSTATEMENT");

            return Ok(new { missionStatement = missionStatement });
        }
    }
}
