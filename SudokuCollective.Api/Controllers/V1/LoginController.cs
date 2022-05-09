using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuCollective.Api.Utilities;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// Login Controller Class
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        private readonly IUserManagementService _userManagementService;
        private readonly IRequestService _requestService;
        private readonly ILogger<LoginController> _logger;

        /// <summary>
        /// Login Controller Constructor
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="userManagementService"></param>
        /// <param name="requestService"></param>
        /// <param name="logger"></param>
        public LoginController(
            IAuthenticateService authService,
            IUserManagementService userManagementService,
            IRequestService requestService,
            ILogger<LoginController> logger)
        {
            _authService = authService;
            _userManagementService = userManagementService;
            _requestService = requestService;
            _logger = logger;
        }

        /// <summary>
        /// An endpoint which issues authorization tokens, does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An authenticated user and a authorization token.</returns>
        /// <response code="200">An authenticated user and a authorization token.</response>
        /// <response code="404">A message detailing any issues logging in the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post endpoint does not require a login. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": string,  // the app license must be valid using the applicable regex pattern as documented in the LoginRequest schema below
        ///       "userName": string, // user name must be unique, the api will ensure this for you; the applicable regex pattern as documented in the LoginRequest schema below
        ///       "password": string, // password is required, the applicable regex pattern as documented in the LoginRequest schema below
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.IsAuthenticatedAsync(request);

                if (result.IsSuccess)
                {
                    result.Message = ControllerMessages.StatusCode200(result.Message);

                    return Ok(result);
                }
                else if (result.Message.Equals(AppsMessages.AppDeactivatedMessage) || 
                    result.Message.Equals(AppsMessages.UserIsNotARegisteredUserOfThisAppMessage))
                {
                    result.Message = ControllerMessages.StatusCode404(result.Message);

                    return NotFound(result);
                }
                else
                {
                    var confirmAuthenticationIssueResponse = await _userManagementService
                        .ConfirmAuthenticationIssueAsync(
                            request.UserName,
                            request.Password,
                            request.License);

                    if (confirmAuthenticationIssueResponse == UserAuthenticationErrorType.USERNAMEINVALID)
                    {
                        result.Message = ControllerMessages.StatusCode404("No User Has This User Name");
                        return NotFound(result);
                    }
                    else if (confirmAuthenticationIssueResponse == UserAuthenticationErrorType.PASSWORDINVALID)
                    {
                        result.Message = ControllerMessages.StatusCode404("Password Invalid");
                        return NotFound(result);
                    }
                    else
                    {
                        return NotFound(ControllerMessages.StatusCode404(result.Message));
                    }
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<LoginController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method which confirms user names for given emails, does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The user name associated with the given email.</returns>
        /// <response code="200">The user name associated with the given email.</response>
        /// <response code="404">A message detailing any issues obtaining the user name.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ConfirmUserNameAsync method does not require a login. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": string, // the applicable regex pattern as documented in the ConfirmUserNameRequest model
        ///       "email": string,   // email is required, the applicable regex pattern is documented in the ConfirmUserNameRequest model
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("ConfirmUserName")]
        public async Task<ActionResult> ConfirmUserNameAsync([FromBody] ConfirmUserNameRequest request)
        {
            try
            {
                var result = await _userManagementService.ConfirmUserNameAsync(
                    request.Email,
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
                return ControllerUtilities.ProcessException<LoginController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
