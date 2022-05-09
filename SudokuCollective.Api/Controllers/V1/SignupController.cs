using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SudokuCollective.Api.Utilities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// Signup Controller Class
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IAuthenticateService _authService;
        private readonly IRequestService _requestService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<SignupController> _logger;

        /// <summary>
        /// Signup Controller Constructor
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="authService"></param>
        /// <param name="requestService"></param>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        public SignupController(
            IUsersService usersService,
            IAuthenticateService authService,
            IRequestService requestService,
            IWebHostEnvironment hostEnvironment, 
            ILogger<SignupController> logger)
        {
            _usersService = usersService;
            _authService = authService;
            _requestService = requestService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// An endpoint which creates new users, does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created user and a authorization token.</returns>
        /// <response code="201">A newly created user and a authorization token.</response>
        /// <response code="404">A message detailing any issues creating the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post endpoint does not require a login. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": string,   // the app license must be valid using the applicable regex pattern as documented in the SignupRequest schema below
        ///       "userName": string,  // user name must be unique, the api will ensure this for you; the applicable regex pattern as documented in the SignupRequest schema below
        ///       "firstName": string, // first name, required and cannot be null but nothing additional to note
        ///       "lastName": string,  // last name, required and cannot be null but nothing additional to note
        ///       "nickName": string,  // nick name, the value can be null but it must be included in the request
        ///       "email": string,     // email must be unique, the api will ensure this for you; the applicable regex pattern as documented in the SignupRequest schema below
        ///       "password": string,  // password is required, the applicable regex pattern as documented in the SignupRequest schema below
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> PostAsync([FromBody] SignupRequest request)
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

                if (_hostEnvironment.IsDevelopment() && !string.IsNullOrEmpty(_hostEnvironment.WebRootPath))
                {
                    emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/create-email-inlined.html");

                    var currentDirectory = string.Format("{0}{1}", Directory.GetCurrentDirectory(), "{0}");

                    emailtTemplatePath = string.Format(currentDirectory, emailtTemplatePath);
                }
                else if (_hostEnvironment.IsStaging())
                {
                    string baseURL = AppDomain.CurrentDomain.BaseDirectory;
                    
                    emailtTemplatePath = string.Format(baseURL + "/Content/EmailTemplates/create-email-inlined.html");
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/create-email-inlined.html";
                }

                var result = await _usersService.CreateAsync(
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

                    var authenticateResult = await _authService.IsAuthenticatedAsync(tokenRequest);

                    if (authenticateResult.IsSuccess)
                    {
                        result.Message = ControllerMessages.StatusCode201(result.Message);
                            
                        result.Payload = new List<object> {
                            new UserCreatedResult
                            { 
                                User = ((AuthenticationResult)authenticateResult.Payload[0]).User,
                                Token = ((AuthenticationResult)authenticateResult.Payload[0]).Token,
                                EmailConfirmationSent = ((EmailConfirmationSentResult)result.Payload[0]).EmailConfirmationSent
                            }};

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
                return ControllerUtilities.ProcessException<SignupController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// An endpoint which resends email confirmations, does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A status detailing the result of processing the request.</returns>
        /// <response code="200">Provides a success message detailing the email confirmation was resent.</response>
        /// <response code="404">A message detailing any issues resending the email confirmation.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ResendEmailConfirmation endpoint does not require a login.  The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": "string", // the app license must be valid using the applicable regex pattern as documented in the ResendEmailConfirmationRequest schema below
        ///       "requestorId": 0,    // the id of the individual requesting the email confirmation is resent
        ///       "appId": 0,          // the id of your app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPut("ResendEmailConfirmation")]
        public async Task<ActionResult> ResendEmailConfirmationAsync([FromBody] ResendEmailConfirmationRequest request)
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

                if (!string.IsNullOrEmpty(_hostEnvironment.WebRootPath))
                {
                    emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/create-email-inlined.html");

                    var currentDirectory = string.Format("{0}{1}", Directory.GetCurrentDirectory(), "{0}");

                    emailtTemplatePath = string.Format(currentDirectory, emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/create-email-inlined.html";
                }

                var result = await _usersService.ResendEmailConfirmationAsync(
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
                return ControllerUtilities.ProcessException<SignupController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
