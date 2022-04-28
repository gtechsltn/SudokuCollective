using System;
using System.Collections.Generic;
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
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.Get(id, request.RequestorId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "name": string,        // name is required, represents the apps name
        ///         "localUrl": string,    // localUrl is not required, an example is https://localhost:8081; regex documented in app schema below
        ///         "stagingUrl": string,  // stagingUrl is not required, an exampled is https://example-app.herokuapp.com; regex documented in app schema below
        ///         "qaUrl": string,       // qaUrl is not required, an exampled is https://example-qa.herokuapp.com; regex documented in app schema below
        ///         "prodUrl": string      // prodUrl is not required, an exampled is https://example-app.com; regex documented in app schema below
        ///         "isActive": boolean    // isActive is required, represents the apps active status
        ///         "environment": integer // environment is required, this integer represents the apps release status: local, staging, qa, or production
        ///         "permitSuperUserAccess": boolean // permitSuperUserAccess is required, indicates if the super user has to register for access
        ///         "permitCollectiveLogins": boolean // permitCollectiveLogins is required, indicates if collective users have to register for access
        ///         "disableCustomUrls": boolean // disableCustomUrls is required, indicates if the app uses custom email and password actions
        ///         "customEmailConfirmationAction": string // customEmailConfirmationAction is required, if implemented this represents the custom action
        ///         "customPasswordResetAction": string // customPasswordResetAction is required, if implemented this represents the custom action
        ///         "timeFrame": integer   // timeFrame is required, represents the timeFrame applied to authorization tokens, if set to years accessDuration is limited to 5
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
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpDelete, Route("{id}")]
        public async Task<ActionResult> Delete(
            int id,
            string license,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(request));

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

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

        /// <summary>
        /// A method which gets an app by its license, available to all roles.
        /// </summary>
        /// <param name="license"></param>
        /// <param name="request"></param>
        /// <returns>Records for a given app.</returns>
        /// <response code="200">Records for a given app.</response>
        /// <response code="404">A message detailing any issues getting an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetByLicense method requires the user to be logged in. Available to all roles. The query parameter license 
        /// refers to the relevant app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPost, Route("{license}/GetByLicense")]
        public async Task<ActionResult<App>> GetByLicense(
            string license,
            [FromBody] Request request)
        {
            try
            {
                if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(request));

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.GetByLicense(license, request.RequestorId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to get a list of all apps, requires superuser or admin roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of all apps.</returns>
        /// <response code="200">A list of all apps.</response>
        /// <response code="404">A message detailing any issues obtaining all apps.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetApps method requires the user to be logged in. Requires superuser or admin roles. The request body 
        /// parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
        /// Sort values are as follows, those applicable to apps are indicated below:
        /// ```
        /// {
        ///     0,  \\ indicates null and is not applicable to apps
        ///     1,  \\ indicates "id" and is applicable to apps
        ///     2,  \\ indicates "userName" and is not applicable to apps
        ///     3,  \\ indicates "firstName" and is not applicable to apps
        ///     4,  \\ indicates "lastName" and is not applicable to apps
        ///     5,  \\ indicates "fullName" and is not applicable to apps
        ///     6,  \\ indicates "nickName" and is not applicable to apps
        ///     7,  \\ indicates "gameCount" and is not applicanle to apps
        ///     8,  \\ indicates "appCount" and is not applicable to apps
        ///     9,  \\ indicates "name" and is applicable to apps
        ///     10, \\ indicates "dateCreated" and is applicable to apps
        ///     11, \\ indicates "dateUpdated" and is applicable to apps
        ///     12, \\ indicates "difficultyLevel" and is not applicable to apps
        ///     13, \\ indicates "userCount" and is applicable to apps
        ///     14  \\ indicates "score" and is not applicable to apps
        /// }
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<App>>> GetApps(
            [FromBody] Request request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.GetApps(request);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to get a list of all apps associated to the signed in user as owner, requires superuser or admin roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of all apps associated to the signed in user as owner.</returns>
        /// <response code="200">A list of all apps associated to the signed in user as owner.</response>
        /// <response code="404">A message detailing any issues obtaining all apps.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetMyApps method requires the user to be logged in. Requires superuser or admin roles. Unlike the above GetApps method
        /// this method specifically gets apps associated with the logged in user as the owner. The request body parameter uses the
        /// request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
        /// Sort values are as follows, those applicable to apps are indicated below:
        /// ```
        /// {
        ///     0,  \\ indicates null and is not applicable to apps
        ///     1,  \\ indicates "id" and is applicable to apps
        ///     2,  \\ indicates "userName" and is not applicable to apps
        ///     3,  \\ indicates "firstName" and is not applicable to apps
        ///     4,  \\ indicates "lastName" and is not applicable to apps
        ///     5,  \\ indicates "fullName" and is not applicable to apps
        ///     6,  \\ indicates "nickName" and is not applicable to apps
        ///     7,  \\ indicates "gameCount" and is not applicanle to apps
        ///     8,  \\ indicates "appCount" and is not applicable to apps
        ///     9,  \\ indicates "name" and is applicable to apps
        ///     10, \\ indicates "dateCreated" and is applicable to apps
        ///     11, \\ indicates "dateUpdated" and is applicable to apps
        ///     12, \\ indicates "difficultyLevel" and is not applicable to apps
        ///     13, \\ indicates "userCount" and is applicable to apps
        ///     14  \\ indicates "score" and is not applicable to apps
        /// }
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPost, Route("GetMyApps")]
        public async Task<ActionResult<IEnumerable<App>>> GetMyApps(
            [FromBody] Request request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService
                        .GetMyApps(
                        request.RequestorId,
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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to get a list of all apps associated to the signed in user as a user, requires superuser or admin roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of all apps associated to the signed in user asa user.</returns>
        /// <response code="200">A list of all apps associated to the signed in user as a user.</response>
        /// <response code="404">A message detailing any issues obtaining all apps.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetMyRegisteredApps method requires the user to be logged in. Requires superuser or admin roles. Unlike the above GetMyApps method
        /// this method specifically gets apps associated with the logged in user as a user. The request body parameter uses the
        /// request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
        /// Sort values are as follows, those applicable to apps are indicated below:
        /// ```
        /// {
        ///     0,  \\ indicates null and is not applicable to apps
        ///     1,  \\ indicates "id" and is applicable to apps
        ///     2,  \\ indicates "userName" and is not applicable to apps
        ///     3,  \\ indicates "firstName" and is not applicable to apps
        ///     4,  \\ indicates "lastName" and is not applicable to apps
        ///     5,  \\ indicates "fullName" and is not applicable to apps
        ///     6,  \\ indicates "nickName" and is not applicable to apps
        ///     7,  \\ indicates "gameCount" and is not applicanle to apps
        ///     8,  \\ indicates "appCount" and is not applicable to apps
        ///     9,  \\ indicates "name" and is applicable to apps
        ///     10, \\ indicates "dateCreated" and is applicable to apps
        ///     11, \\ indicates "dateUpdated" and is applicable to apps
        ///     12, \\ indicates "difficultyLevel" and is not applicable to apps
        ///     13, \\ indicates "userCount" and is applicable to apps
        ///     14  \\ indicates "score" and is not applicable to apps
        /// }
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPost, Route("GetMyRegisteredApps")]
        public async Task<ActionResult> GetMyRegisteredApps(
            [FromBody] Request request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.GetMyRegisteredApps(
                        request.RequestorId,
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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to get a list of all users registered to an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A list of all users registered to an app.</returns>
        /// <response code="200">A list of all users registered to an app.</response>
        /// <response code="404">A message detailing any issues obtaining all users.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetAppUsers method requires the user to be logged in. Requires superuser or admin roles. Returns a list of all users 
        /// registered to an app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
        [HttpPost, Route("{id}/GetAppUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAppUsers(
            int id,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService
                        .GetAppUsers(
                            id,
                            request.RequestorId,
                            request.Paginator,
                            true);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to get a list of all users not registered to an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A list of all users not registered to an app.</returns>
        /// <response code="200">A list of all users not registered to an app.</response>
        /// <response code="404">A message detailing any issues obtaining all users.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetAppUsers method requires the user to be logged in. Requires superuser or admin roles. Returns a list of all users 
        /// not registered to an app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
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
        [HttpPost, Route("{id}/GetNonAppUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetNonAppUsers(
            int id,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService
                        .GetAppUsers(
                            id,
                            request.RequestorId,
                            request.Paginator,
                            false);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method which adds a user to an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns>A user added to an app.</returns>
        /// <response code="200">A user added to an app.</response>
        /// <response code="404">A message detailing any issues adding a user to an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The AddUser method requires the user to be logged in. Requires superuser or admin roles. The query parameter id 
        /// refers to the relevant app and the query parameter userId refers to the relevant user. The request body parameter 
        /// uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/AddUser")]
        public async Task<IActionResult> AddUser(
            int id,
            int userId,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (userId == 0) throw new ArgumentException(ControllerMessages.UserIdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.AddAppUser(id, userId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method which removes a user from an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns>A message detailing a user has been removed from an app.</returns>
        /// <response code="200">A message detailing a user has been removed from an app.</response>
        /// <response code="404">A message detailing any issues removing a user from an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The RemoveUser method requires the user to be logged in. Requires superuser or admin roles. The query parameter id 
        /// refers to the relevant app and the query parameter userId refers to the relevant user. The request body parameter 
        /// uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/RemoveUser")]
        public async Task<IActionResult> RemoveUser(
            int id,
            int userId,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (userId == 0) throw new ArgumentException(ControllerMessages.UserIdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.RemoveAppUser(id, userId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to activate an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A message detailing if an app has been activated.</returns>
        /// <response code="200">A message detailing if an app has been activated.</response>
        /// <response code="404">A message detailing any issues activating an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Activate method requires the user to be logged in. Requires superuser or admin roles. The query parameter id 
        /// refers to the relevant app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/Activate")]
        public async Task<IActionResult> Activate(
            int id, 
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.Activate(id);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to deactivate an app, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A message detailing if an app has been deactivated.</returns>
        /// <response code="200">A message detailing if an app has been deactivated.</response>
        /// <response code="404">A message detailing any issues deactivating an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Deactivate method requires the user to be logged in. Requires superuser or admin roles. The query parameter id 
        /// refers to the relevant app. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/Deactivate")]
        public async Task<IActionResult> Deactivate(
            int id,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.Deactivate(id);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to reset apps, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A reset app with all games deleted.</returns>
        /// <response code="200">A message documenting the result of the reset request.</response>
        /// <response code="404">A message detailing any issues resetting an app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Reset method requires the user to be logged in. Requires superuser or admin roles. Returns a copy of
        /// the app with all games deleted. The query parameters id refers to the relevant app. The request body 
        /// parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/Reset")]
        public async Task<ActionResult> Reset(
            int id,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsUserOwnerOThisfApp(
                    _httpContextAccessor,
                    request.License,
                    request.RequestorId,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.DeleteOrReset(id, true);

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

        /// <summary>
        /// A method to add admin privileges to a given user, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns>A copy of the user with admin privileges added for the given app.</returns>
        /// <response code="200">A message documenting the result of the promotion request.</response>
        /// <response code="404">A message detailing any issues promoting a user to an admin for a given app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The ActivateAdminPrivileges method requires the user to be logged in. Requires superuser or admin roles. Returns a copy of
        /// the relevant user with admin privileges added. The query parameters id refers to the relevant app. The request body 
        /// parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpPut, Route("{id}/ActivateAdminPrivileges")]
        public async Task<ActionResult> ActivateAdminPrivileges(
            int id,
            int userId,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (userId == 0) throw new ArgumentException(ControllerMessages.UserIdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.ActivateAdminPrivileges(id, userId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
        /// A method to remove admin privileges from a given user, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns>A copy of the user with admin privileges removed for the given app.</returns>
        /// <response code="200">A message documenting the result of the demotion request.</response>
        /// <response code="404">A message detailing any issues demoting a user from an admin for a given app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The DeactivateAdminPrivileges method requires the user to be logged in. Requires superuser or admin roles. Returns a copy of
        /// the relevant user with admin privileges removed. The query parameters id refers to the relevant app. The request body 
        /// parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {}           // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPut, Route("{id}/DeactivateAdminPrivileges")]
        public async Task<ActionResult> DeactivateAdminPrivileges(
            int id,
            int userId,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (userId == 0) throw new ArgumentException(ControllerMessages.UserIdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisToken(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _appsService.DeactivateAdminPrivileges(id, userId);

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
                    return ControllerUtilities.ProcessTokenError(this);
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
