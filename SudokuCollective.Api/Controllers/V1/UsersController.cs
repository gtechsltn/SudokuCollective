using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuCollective.Api.Utilities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// User Controller Class
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IAppsService _appsService;
        private readonly IRequestService _requestService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// User Controller Constructor
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="appsService"></param>
        /// <param name="requestService"></param>
        /// <param name="hostEnvironment"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public UsersController(
            IUsersService usersService,
            IAppsService appsService,
            IRequestService requestService,
            IWebHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _appsService = appsService;
            _requestService = requestService;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// A method which gets a given user relative to the requesting app, available to all roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>Records for a given user relative to the requesting app.</returns>
        /// <response code="200">Records for a given user relative to the given app.</response>
        /// <response code="404">A message detailing any issues getting a user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Get method requires the user to be logged in. Available to all roles. The query parameter id 
        /// refers to the relevant user. The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPost("{id}")]
        public async Task<ActionResult<User>> Get(
            int id,
            [FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.Get(
                        id,
                        request.License,
                        request);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to update users, available to all roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>An updated user.</returns>
        /// <response code="200">An updated user.</response>
        /// <response code="404">A message detailing any issues updating a user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Update method requires the user to be logged in. Available to all roles. The query 
        /// parameter id refers to the relevant user. The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "userName": string,  // username is required; uses the same regex pattern as documented in the LoginRequest model
        ///         "firstName": string, // firstname is required
        ///         "lastName": string,  // lastname is required
        ///         "nickName": string,  // nickname is not required
        ///         "email": string      // email is required; uses the same regex pattern as documented in the SignupRequest model
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, 
            [FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
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
                        emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/confirm-old-email-inlined.html");

                        var currentDirectory = string.Format("{0}{1}", Directory.GetCurrentDirectory(), "{0}");

                        emailtTemplatePath = string.Format(currentDirectory, emailtTemplatePath);
                    }
                    else
                    {
                        emailtTemplatePath = "../../Content/EmailTemplates/confirm-old-email-inlined.html";
                    }

                    var result = await _usersService.Update(
                        id,
                        request,
                        baseUrl,
                        emailtTemplatePath);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to delete users, available to all roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A message documenting the result of the delete request.</returns>
        /// <response code="200">A message documenting the result of the delete request.</response>
        /// <response code="404">A message detailing any issues deleting a user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Delete method requires the user to be logged in. Available to all roles. The query parameter 
        /// id refers to the relevant user. The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(
            int id, 
            [FromBody] Request request)
        {
            _requestService.Update(request);
            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.Delete(id, request.License);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to get a list of all users, requires superuser or admin roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of all users.</returns>
        /// <response code="200">A list of all users.</response>
        /// <response code="404">A message detailing any issues obtaining all users.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetUsers method requires the user to be logged in. Requires superuser or admin roles. The request body 
        /// parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": {
        ///         "page": integer,                 // this param works in conjection with itemsPerPage starting with page 1
        ///         "itemsPerPage": integer          // in conjunction with page if you want items 11 through 21 page would be 2 and this would be 10
        ///         "sortBy": sortValue              // an enumeration indicating the field for sorting, documented below; you return the integer
        ///         "OrderByDescending": boolean     // a boolean to indicate is the order is ascending or descending
        ///         "includeCompletedGames": boolean // a boolean which only applies to game lists
        ///       },
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        ///
        /// Sort values are as follows, those applicable to users are indicated below:
        /// ```
        /// {
        ///     0,  \\ indicates null and is not applicable to users
        ///     1,  \\ indicates "id" and is applicable to users
        ///     2,  \\ indicates "userName" and is applicable to users
        ///     3,  \\ indicates "firstName" and is applicable to users
        ///     4,  \\ indicates "lastName" and is applicable to users
        ///     5,  \\ indicates "fullName" and is applicable to users
        ///     6,  \\ indicates "nickName" and is applicable to users
        ///     7,  \\ indicates "gameCount" and is applicanle to users
        ///     8,  \\ indicates "appCount" and is applicable to users
        ///     9,  \\ indicates "name" and is not applicable to users
        ///     10, \\ indicates "dateCreated" and is applicable to users
        ///     11, \\ indicates "dateUpdated" and is applicable to users
        ///     12, \\ indicates "difficultyLevel" and is not applicable to users
        ///     13, \\ indicates "userCount" and is not applicable to users
        ///     14  \\ indicates "score" and is not applicable to users
        /// }
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.GetUsers(
                        request.RequestorId,
                        request.License,
                        request.Paginator);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to add roles to a user for a given app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>An updated copy of the user with the added role.</returns>
        /// <response code="200">An updated copy of the user with the added role.</response>
        /// <response code="404">A message detailing any issues adding the role to the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The AddRoles method requires the user to be logged in. Requires superuser or admin roles. The query 
        /// parameter id refers to the relevant user. The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "roleIds": Array[integer],  // an array of integers for the roles you wish to add
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut("{id}/AddRoles")]
        public async Task<IActionResult> AddRoles(
            int id,
            [FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.AddUserRoles(
                        id,
                        request,
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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to remove roles from a user for a given app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>An updated copy of the user with the removed role.</returns>
        /// <response code="200">An updated copy of the user with the removed role.</response>
        /// <response code="404">A message detailing any issues adding the role to the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The RemoveRoles method requires the user to be logged in. Requires superuser or admin roles. The query 
        /// parameter id refers to the relevant user. The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "roleIds": Array[integer],  // an array of integers for the roles you wish to remove
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut("{id}/RemoveRoles")]
        public async Task<IActionResult> RemoveRoles(
            int id,
            [FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.RemoveUserRoles(
                        id,
                        request,
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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to activate a user, requires the superuser role.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An updated copy of the user with the new active status.</returns>
        /// <response code="200">An updated copy of the user with the new active status.</response>
        /// <response code="404">A message detailing any issues activating the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Activate method requires the user to be logged in. Requires the superuser role.
        /// </remarks>
        [Authorize(Roles = "SUPERUSER")]
        [HttpPut("{id}/Activate")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var result = await _usersService.Activate(id);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to deactivate a user, requires the superuser role.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An updated copy of the user with the new active status.</returns>
        /// <response code="200">An updated copy of the user with the new active status.</response>
        /// <response code="404">A message detailing any issues deactivating the user.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Deactivate method requires the user to be logged in. Requires the superuser role.
        /// </remarks>
        [Authorize(Roles = "SUPERUSER")]
        [HttpPut("{id}/Deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var result = await _usersService.Deactivate(id);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to reset user password, your custom password action will plug into this endpoint; does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An updated copy of the user.</returns>
        /// <response code="200">An updated copy of the user.</response>
        /// <response code="404">A message detailing any issues resetting the users password.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ResetPassword method does not require a login. This method works in conjunction with the custom email action for your 
        /// app. When your custom action is ready to submit it's data it will submit the token it receives from the email along with the 
        /// new password to this endpoint. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "token": string,      // this will be provided by the api, the applicable regex pattern is documented in the ResetPasswordRequest model
        ///       "newPassword": string // the new password, the applicable regex pattern is documented in the ResetPasswordRequest model
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var user = (User)(await _usersService.GetUserByPasswordToken(request.Token)).Payload[0];
                var license = (await _usersService.GetAppLicenseByPasswordToken(request.Token)).License;

                var updatePasswordRequest = new UpdatePasswordRequest
                {
                    UserId = user.Id,
                    NewPassword = request.NewPassword,
                    License = license
                };

                var result = await _usersService.UpdatePassword(updatePasswordRequest);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to request that a password reset email; does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A boolean indicating if the password reset email was sent.</returns>
        /// <response code="200">A boolean indicating if the password reset email was sent.</response>
        /// <response code="404">A message detailing any issues sending the password reset email.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The RequestPasswordReset method does not require a login. It sends password reset emails. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string, // the app license must be valid using the applicable regex pattern as documented in the RequestPasswordResetRequest model
        ///       "email": string    // email is required, the applicable regex pattern is documented in the RequestPasswordResetRequest model
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
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
                    emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/password-reset-requested-inlined.html");

                    emailtTemplatePath = string.Format("../SudokuCollective.Api{0}", emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/confirm-old-email-inlined.html";
                }

                var result = await _usersService.RequestPasswordReset(request, baseUrl, emailtTemplatePath);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to request that a password reset email be resent; does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A boolean indicating if the password reset email was resent.</returns>
        /// <response code="200">A boolean indicating if the password reset email was resent.</response>
        /// <response code="404">A message detailing any issues resending the password reset email.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ResendRequestPasswordReset method does not require a login. It resends password reset emails
        /// if the user has lost the original email. The request body parameter uses a custom request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "userId": integer, // the id for the requesting user
        ///       "appId": integer   // the id for the relevant app
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPut("ResendRequestPasswordReset")]
        public async Task<IActionResult> ResendRequestPasswordReset([FromBody] ResendRequestPasswordRequest request)
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
                    emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/password-reset-requested-inlined.html");

                    emailtTemplatePath = string.Format("../SudokuCollective.Api{0}", emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/confirm-old-email-inlined.html";
                }

                var result = await _usersService.ResendPasswordReset(
                    request.UserId,
                    request.AppId,
                    baseUrl,
                    emailtTemplatePath);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method which confirms user emails, does not require a login.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>A boolean indicating if the email was confirmed.</returns>
        /// <response code="200">A boolean indicating if the email was confirmed.</response>
        /// <response code="404">A message detailing any issues confirming the users email.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ConfirmEmail method does not require a login. If you've implemented a custom confirm email action that action will link
        /// back to this endpoint once confirmed. Your action will send the token to this endpoint to complete the process, the boolean 
        /// will indicate if the email was confirmed.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("ConfirmEmail/{token}")]
        public async Task<IActionResult> ConfirmEmail(string token)
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
                    emailtTemplatePath = Path.Combine(_hostEnvironment.WebRootPath, "/Content/EmailTemplates/confirm-new-email-inlined.html");

                    emailtTemplatePath = string.Format("../SudokuCollective.Api{0}", emailtTemplatePath);
                }
                else
                {
                    emailtTemplatePath = "../../Content/EmailTemplates/confirm-new-email-inlined.html";
                }

                var result = await _usersService.ConfirmEmail(token, baseUrl, emailtTemplatePath);

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
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method which cancels oustanding email confirmation requests, available to all roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A boolean indicating if the outstanding email confirmation was cancelled.</returns>
        /// <response code="200">A boolean indicating if the outstanding email confirmation was cancelled.</response>
        /// <response code="404">A message detailing any issues cancelling outstanding email confirmation requests.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The CancelEmailConfirmation method requires the user to be logged in.  The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPut, Route("cancelEmailConfirmation")]
        public async Task<IActionResult> CancelEmailConfirmation([FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.CancelEmailConfirmationRequest(request.RequestorId, request.AppId);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method which cancels oustanding password reset requests, available to all roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A boolean indicating if the outstanding password reset request was cancelled.</returns>
        /// <response code="200">A boolean indicating if the outstanding email confirmations were cancelled.</response>
        /// <response code="404">A message detailing any issues cancelling outstanding email confirmation requests.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The CancelPasswordReset method requires the user to be logged in.  The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPut, Route("cancelPasswordReset")]
        public async Task<IActionResult> CancelPasswordReset([FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.CancelPasswordResetRequest(request.RequestorId, request.AppId);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method which cancels oustanding email confirmation and password reset requests, available to all roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A boolean indicating if outstanding email confirmation and password reset requests were cancelled.</returns>
        /// <response code="200">A boolean indicating if outstanding email confirmation and password reset requests were cancelled.</response>
        /// <response code="404">A message detailing any issues cancelling outstanding email confirmation and password reset requests.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The CancelAllEmailRequests method requires the user to be logged in.  The request body parameter users the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPut, Route("cancelAllEmailRequests")]
        public async Task<IActionResult> CancelAllEmailRequests([FromBody] Request request)
        {
            _requestService.Update(request);

            try
            {
                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _usersService.CancelAllEmailRequests(request.RequestorId, request.AppId);

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
                else
                {
                    var result = new Result
                    {
                        IsSuccess = false,
                        Message = ControllerMessages.InvalidTokenRequestMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<UsersController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
