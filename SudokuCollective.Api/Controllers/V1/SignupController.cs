using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// Signup Controller Class
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly IAuthenticateService authService;
        private readonly IWebHostEnvironment hostEnvironment;

        /// <summary>
        /// Signup Controller Constructor
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
        /// A method which creates new users.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created user and a authorization token.</returns>
        /// <response code="201">A newly created user and a authorization token.</response>
        /// <response code="404">A message detailing any issues creating the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post method does not require an authorization token. The method uses uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "userName": "string",  // user name must be unique, the api will ensure this for you; the applicable regex pattern as documented in the SignupRequest model
        ///       "firstName": "string", // first name, required and cannot be null but nothing additional to note
        ///       "lastName": "string",  // last name, required and cannot be null but nothing additional to note
        ///       "nickName": "string",  // nick name, the value can be null but it must be included in the request
        ///       "email": "string",     // email must be unique, the api will ensure this for you; the applicable regex pattern as documented in the SignupRequest model
        ///       "password": "string",  // password is required, the applicable regex pattern as documented in the SignupRequest model
        ///       "license": "string"    // the app license must be valid using the applicable regex pattern as documented in the SignupRequest model
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

                if (hostEnvironment.IsDevelopment() && !string.IsNullOrEmpty(hostEnvironment.WebRootPath))
                {
                    emailtTemplatePath = Path.Combine(hostEnvironment.WebRootPath, "/Content/EmailTemplates/create-email-inlined.html");

                    var currentDirectory = string.Format("{0}{1}", Directory.GetCurrentDirectory(), "{0}");

                    emailtTemplatePath = string.Format(currentDirectory, emailtTemplatePath);
                }
                else if (hostEnvironment.IsStaging())
                {
                    string baseURL = AppDomain.CurrentDomain.BaseDirectory;
                    
                    emailtTemplatePath = string.Format(baseURL + "/Content/EmailTemplates/create-email-inlined.html");
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
        /// A method which resends email confirmations.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A status detailing the result of processing the request.</returns>
        /// <response code="200">Provides a success message detailing the email confirmation was resent.</response>
        /// <response code="404">A message detailing any issues resending the email confirmation.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ResendEmailConfirmation method does not require an authorization token.  The method uses uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": "string", // the app license must be valid using the applicable regex pattern as documented in the ResendEmailConfirmationRequest model
        ///       "requestorId": 0,    // the id of the individual requesting the email confirmation is resent
        ///       "appId": 0           // the id of your app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPut("ResendEmailConfirmation")]
        public async Task<ActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequest request)
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

                    var currentDirectory = string.Format("{0}{1}", Directory.GetCurrentDirectory(), "{0}");

                    emailtTemplatePath = string.Format(currentDirectory, emailtTemplatePath);
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
