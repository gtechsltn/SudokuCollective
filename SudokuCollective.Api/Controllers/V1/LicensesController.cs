using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuCollective.Api.Utilities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// License Controller Class
    /// </summary>
    [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
    [Route("api/[controller]")]
    [ApiController]
    public class LicensesController : ControllerBase
    {
        private readonly IAppsService _appsService;
        private readonly IRequestService _requestService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LicensesController> _logger;

        /// <summary>
        /// App Controller Constructor
        /// </summary>
        /// <param name="appsService"></param>
        /// <param name="requestService"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public LicensesController(
            IAppsService appsService,
            IRequestService requestService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LicensesController> logger)
        {
            _appsService = appsService;
            _requestService = requestService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// A method to create app licenses, requires superuser or admin roles.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An new app.</returns>
        /// <response code="200">An new app.</response>
        /// <response code="404">A message detailing any issues creating a new app.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Post method requires the user to be logged in. Requires superuser or admin roles. The
        /// request body parameter uses the request model.
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
        ///         "ownerId": integer     // ownerId is required, represents the signed in users id
        ///         "localUrl": string,    // localUrl is not required, an example is https://localhost:8081
        ///         "stagingUrl": string,  // stagingUrl is not required, an exampled is https://example-app.herokuapp.com
        ///         "qaUrl": string,       // qaUrl is not required, an exampled is https://example-qa.herokuapp.com
        ///         "prodUrl": string      // prodUrl is not required, an exampled is https://example-app.com
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPost]
        public async Task<ActionResult<App>> Post(
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
                    var result = await _appsService.Create(request);

                    if (result.IsSuccess)
                    {
                        result.Message = ControllerMessages.StatusCode201(result.Message);

                        return StatusCode((int)HttpStatusCode.Created, result);
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
                return ControllerUtilities.ProcessException<LicensesController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// A method to obtain an apps license, requires superuser or admin roles.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A license for a given app.</returns>
        /// <response code="200">A license for a given app.</response>
        /// <response code="404">A message detailing any issues getting an app license.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Get method requires the user to be logged in. Requires superuser or admin roles. The query parameter id 
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
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpGet, Route("{id}")]
        public async Task<ActionResult> Get(
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
                    var result = await _appsService.GetLicense(id, request.RequestorId);

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
                return ControllerUtilities.ProcessException<LicensesController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
