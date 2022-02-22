using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Api.Controllers
{
    /// <summary>
    /// Hello World Controller Class
    /// </summary>
    [AllowAnonymous]
    [Route("api/helloWorld")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        /// <summary>
        /// An example method of how to connect to an API for new developers.
        /// </summary>
        /// <returns>A simple message outlining the response model, if a param was included it is echoed in the response.</returns>
        /// <response code="200">A simple message outlining the response model, if a param was included it is echoed in the response.</response>
        /// <remarks>
        /// The Get method does not require an authorization token.  This method serves as a simple example of how to connect to an 
        /// api for new developers.  The method accepts a query parameter of 'param', if param is empty it issues a canned response.
        /// 
        /// An example request is as follows: 
        /// ```
        /// https://sudokucollective.com/api/helloWorld?param=hello_world
        /// ```
        /// The standard API response is as follows:
        /// ```
        /// {
        ///   "isSuccess": true,                                        // An indicator if the request was successful
        ///   "isFromCache": false,                                     // An indicator if the payload was obtained from the cache
        ///   "message": "Status Code 200: You Submitted: hello_world", // A brief description of the result
        ///   "payload": []                                             // The payload response you requested, returned as an array of objects
        /// }
        /// ```
        /// </remarks>
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
