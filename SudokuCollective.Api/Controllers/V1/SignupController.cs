using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// The signup controller.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly IAuthenticateService authService;
        private readonly IWebHostEnvironment hostEnvironment;

        /// <summary>
        /// Sign up controller constructor.
        /// </summary>
        /// <param name="usersServ"></param>
        /// <param name="authServ"></param>
        /// <param name="environment"></param>
        public SignupController(
            IUsersService usersServ,
            IAuthenticateService authServ,
            IWebHostEnvironment environment)
        {
            usersService = usersServ;
            authService = authServ;
            hostEnvironment = environment;
        }

        /// <summary>
        /// A method to create new users.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created user and a authorization token.</returns>
        /// <response code="201">A newly created user and a authorization token.</response>
        /// <response code="404">A message detailing any issues creating the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post method is an annonymous method, it doesn't require an authorization token, that uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "userName": "string",  // user name must be unique, the api will ensure this for you, and the applicable regex pattern is documented in the SignupRequest model
        ///       "firstName": "string", // first name, required but nothing special to note
        ///       "lastName": "string",  // last name, required but nothing special to note
        ///       "nickName": "string",  // nick name, the value can be null but it must be included in the request
        ///       "email": "string",     // email must be unique, the api will ensure this for you, and the applicable regex pattern is documented in the SignupRequest model
        ///       "password": "string",  // password is required and the applicable regex pattern is documented in the SignupRequest model
        ///       "license":"string",    // license of your app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] SignupRequest request)
        {
            try
            {
                string baseUrl;

                if (Request != null)
                {
                    baseUrl = Request.Host.ToString();
                }
                else
                {
                    baseUrl = "https://SudokuCollective.com";
                }

                string emailtTemplatePath;

                if (!string.IsNullOrEmpty(hostEnvironment.WebRootPath))
                {
                    emailtTemplatePath = Path.Combine(hostEnvironment.WebRootPath, "/Content/EmailTemplates/create-email-inlined.html");

                    emailtTemplatePath = string.Format("../SudokuCollective.Api{0}", emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/create-email-inlined.html";
                }

                var result = await usersService.Create(
                    request,
                    baseUrl,
                    emailtTemplatePath);

                if (result.IsSuccess)
                {
                    var tokenRequest = new LoginRequest
                    {
                        UserName = request.UserName,
                        Password = request.Password,
                        License = request.License
                    };

                    var authenticateResult = await authService.IsAuthenticated(tokenRequest);

                    if (authenticateResult.IsSuccess)
                    {
                        result.Message = ControllerMessages.StatusCode201(result.Message);
                        result.Payload = authenticateResult.Payload;

                        return StatusCode((int)HttpStatusCode.Created, result);
                    }
                    else
                    {
                        result.Message = ControllerMessages.StatusCode404(authenticateResult.Message);

                        return NotFound(result);
                    }
                }
                else
                {
                    result.Message = ControllerMessages.StatusCode404(result.Message);

                    return NotFound(result);
                }
            }
            catch (Exception e)
            {
                var result = new Result
                {
                    IsSuccess = false,
                    Message = ControllerMessages.StatusCode500(e.Message)
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }
        }

        /// <summary>
        /// A method to resend email confirmations
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A status detailing the result of processing the request.</returns>
        /// <response code="200">Provides a success message detailing the email confirmation was resent.</response>
        /// <response code="404">A message detailing any issues resending the email confirmation.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ResendEmailConfirmation method is an annonymous method, it doesn't require an authorization token, 
        /// that uses the standard request model, the paginator and payload are not required.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": "string",            // license of your app
        ///       "requestorId": 0,               // id for the user this request pertains to     
        ///       "appId": 0,                     // id of your app                
        ///       "paginator": {                  
        ///         "page": 0,                    // not applicable             
        ///         "itemsPerPage": 0,            // not applicable       
        ///         "sortBy": 0,                  // not applicable              
        ///         "orderByDescending": true,    // not applicable
        ///         "includeCompletedGames": true // not applicable
        ///       }                               
        ///       "payload": {}                   // not applicable               
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPut("ResendEmailConfirmation")]
        public async Task<ActionResult> ResendEmailConfirmation([FromBody] Request request)
        {
            try
            {
                string baseUrl;

                if (Request != null)
                {
                    baseUrl = Request.Host.ToString();
                }
                else
                {
                    baseUrl = "https://SudokuCollective.com";
                }

                string emailtTemplatePath;

                if (!string.IsNullOrEmpty(hostEnvironment.WebRootPath))
                {
                    emailtTemplatePath = Path.Combine(hostEnvironment.WebRootPath, "/Content/EmailTemplates/create-email-inlined.html");

                    emailtTemplatePath = string.Format("../SudokuCollective.Api{0}", emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/create-email-inlined.html";
                }

                var result = await usersService.ResendEmailConfirmation(
                    request.RequestorId,
                    request.AppId,
                    baseUrl,
                    emailtTemplatePath,
                    request.License);

                if (result.IsSuccess)
                {
                    result.Message = ControllerMessages.StatusCode200(result.Message);

                    return Ok(result);
                }
                else
                {
                    result.Message = ControllerMessages.StatusCode404(result.Message);

                    return NotFound(result);
                }
            }
            catch (Exception e)
            {
                var result = new Result
                {
                    IsSuccess = false,
                    Message = ControllerMessages.StatusCode500(e.Message)
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }
        }
    }
}
