using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var result = new BaseResult
            {
                IsSuccess = true,
                Message = "Status Code 200: Hello World from Sudoku Collective!"
            };

            return Ok(result);
        }
    }
}

public class BaseResult
{
    public bool IsSuccess { get; set; }
    public bool IsFromCache { get; set; }
    public string Message { get; set; }

    public BaseResult()
    {
        IsSuccess = false;
        IsFromCache = false;
        Message = string.Empty;
    }
}