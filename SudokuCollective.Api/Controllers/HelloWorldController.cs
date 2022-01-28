using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;

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
                Message = ControllerMessages.StatusCode200(ControllerMessages.HelloWorld)
            };

            return Ok(result);
        }
    }
}
