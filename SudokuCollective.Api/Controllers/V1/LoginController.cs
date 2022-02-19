using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// The login controller.
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticateService authService;
        private readonly IUserManagementService userManagementService;

        /// <summary>
        /// Login controller constructor.
        /// </summary>
        /// <param name="authServ"></param>
        /// <param name="userManagementServ"></param>
        public LoginController(
            IAuthenticateService authServ,
            IUserManagementService userManagementServ)
        {
            authService = authServ;
            userManagementService = userManagementServ;
        }

        /// <summary>
        /// A method to login to the api.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An authenticated user and a authorization token.</returns>
        /// <response code="200">An authenticated user and a authorization token.</response>
        /// <response code="404">A message detailing any issues logging in the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post method is an annonymous method, it doesn't require an authorization token, that uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "userName": "string", // user name must be unique, the api will ensure this for you, and the applicable regex pattern is documented in the LoginRequest model
        ///       "password": "string", // password is required and the applicable regex pattern is documented in the LoginRequest model
        ///       "license":"string",   // license of your app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LoginRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await authService.IsAuthenticated(request);

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
                    var confirmAuthenticationIssueResponse = await userManagementService
                        .ConfirmAuthenticationIssue(
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
                        return NotFound(ControllerMessages.StatusCode404("Bad Request"));
                    }
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
        /// A method to confirm user names for given emails.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The user name associated with the given email.</returns>
        /// <response code="200">The user name associated with the given email.</response>
        /// <response code="404">A message detailing any issues obtaining the user name.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ConfirmUserName method is an annonymous method, it doesn't require an authorization token, that uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "email": "string",  // password is required and the applicable regex pattern is documented in the ConfirmUserNameRequest model
        ///       "license":"string", // license of your app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("ConfirmUserName")]
        public async Task<ActionResult> ConfirmUserName([FromBody] ConfirmUserNameRequest request)
        {
            try
            {
                var result = await userManagementService.ConfirmUserName(
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
