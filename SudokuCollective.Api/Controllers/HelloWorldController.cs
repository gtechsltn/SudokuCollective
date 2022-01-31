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
        public IActionResult Get(string param = null)
        {
            var result = new Result
            {
                IsSuccess = true,
            };

            if (string.IsNullOrEmpty(param))
            {
                result.Message = ControllerMessages.StatusCode200(ControllerMessages.HelloWorld);
            }
            else
            {
                result.Message = ControllerMessages.StatusCode200(ControllerMessages.Echo(param));
            }

            return Ok(result);
        }
    }
}
