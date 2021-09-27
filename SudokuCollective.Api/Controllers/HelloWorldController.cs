using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/helloWorld")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var result = new Result
            {
                IsSuccess = true,
                Message = "Status Code 200: Hello World from Sudoku Collective!"
            };

            return Ok(result);
        }
    }
}
