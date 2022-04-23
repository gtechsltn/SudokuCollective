using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuCollective.Api.Utilities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// App Controller Class
    /// </summary>
    [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private readonly IAppsService _appsService;
        private readonly IRequestService _requestService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AppsController> _logger;

        /// <summary>
        /// App Controller Constructor
        /// </summary>
        /// <param name="appsService"></param>
        /// <param name="requestService"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public AppsController(
            IAppsService appsService,
            IRequestService requestService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AppsController> logger)
        {
            _appsService = appsService;
            _requestService = requestService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        
        /// <summary>
        /// A method which gets an app, available to all roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>Records for a given app.</returns>
        /// <response code="200">Records for a given app.</response>
        /// <response code="404">A message detailing any issues getting an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Get method requires the user to be logged in. Available to all roles. The query parameter id 
        /// refers to the relevant app. The request body parameter uses the request model.
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
        [HttpPost, Route("{id}")]
        public async Task<ActionResult<App>> Get(
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
                    var result = await _appsService.Get(id);

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
                return ControllerUtilities.ProcessException<AppsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to update apps, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>An updated app.</returns>
        /// <response code="200">An updated app.</response>
        /// <response code="404">A message detailing any issues updating a app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Update method requires the user to be logged in. Requires superuser or admin roles. The query 
        /// parameter id refers to the relevant app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request model
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "name": string,        // name is required, represents the apps name
        ///         "localUrl": string,    // localUrl is not required, an example is https://localhost:8081
        ///         "stagingUrl": string,  // stagingUrl is not required, an exampled is https://example-app.herokuapp.com
        ///         "qaUrl": string,       // qaUrl is not required, an exampled is https://example-qa.herokuapp.com
        ///         "prodUrl": string      // prodUrl is not required, an exampled is https://example-app.com
        ///         "isActive": boolean    // isActive is required, represents the apps active status
        ///         "environment": integer // environment is required, this integer represents the apps release status: local, staging, qa, or production
        ///         "permitSuperUserAccess": boolean // permitSuperUserAccess is required, indicates if the super user has to register for access
        ///         "permitCollectiveLogins": boolean // permitCollectiveLogins is required, indicates if collective users have to register for access
        ///         "disableCustomUrls": boolean // disableCustomUrls is required, indicates if the app uses custom email and password actions
        ///         "customEmailConfirmationAction": string // customEmailConfirmationAction is required, if implemented this represents the custom action
        ///         "customPasswordResetAction": string // customPasswordResetAction is required, if implemented this represents the custom action
        ///         "timeFrame": integer   // timeFrame is required, represents the timeFrame applied to authorization tokens
        ///         "accessDuration": integer // accessDuration is required, represents the magnitude of the timeframe: eq: 1 day
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}")]
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
                    var result = await _appsService.Update(id, request);

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
                return ControllerUtilities.ProcessException<AppsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
        
        /// <summary>
        /// A method to delete apps, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="license"></param>
        /// <param name="request"></param>
        /// <returns>A message documenting the result of the delete request.</returns>
        /// <response code="200">A message documenting the result of the delete request.</response>
        /// <response code="404">A message detailing any issues deleting an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Delete method requires the user to be logged in. Requires superuser or admin roles. The query parameters 
        /// id and license refers to the relevant app. The request body parameter uses the request model.
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
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpDelete, Route("{id}/{license}")]
        public async Task<ActionResult> Delete(
            int id,
            string license,
            [FromBody] Request request)
        {
            try
            {
                if (await _appsService.IsUserOwnerOThisfApp(
                    _httpContextAccessor,
                    license,
                    request.RequestorId,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.DeleteOrReset(id);

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
                        Message = ControllerMessages.NotOwnerMessage
                    };

                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return ControllerUtilities.ProcessException<AppsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
