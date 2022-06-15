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
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Api.V1.Controllers
{
    /// <summary>
    /// Solutions Controller Class
    /// </summary>
    [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SolutionsController : ControllerBase
    {
        private readonly ISolutionsService _solutionsService;
        private readonly IAppsService _appsService;
        private readonly IRequestService _requestService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SolutionsController> _logger;

        /// <summary>
        /// Solutions Controller Constructor
        /// </summary>
        /// <param name="solutionsService"></param>
        /// <param name="appsService"></param>
        /// <param name="requestService"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public SolutionsController(
            ISolutionsService solutionsService,
            IAppsService appsService,
            IRequestService requestService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SolutionsController> logger
            )
        {
            _solutionsService = solutionsService;
            _appsService = appsService;
            _requestService = requestService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// An endpoint to get a solution, requires the user role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>A solution.</returns>
        /// <response code="200">A solution.</response>
        /// <response code="404">A message detailing any issues getting a solution.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Get endpoint requires the user to be logged in. Requires the user role. The query parameter id refers to the relevant solution. 
        /// The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {},          // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "USER")]
        [HttpPost("{id}")]
        public async Task<ActionResult<SudokuSolution>> GetAsync(
            int id,
            [FromBody] Request request)
        {
            try
            {
                if (id == 0) throw new ArgumentException(ControllerMessages.IdCannotBeZeroMessage);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _requestService.Update(request);

                if (await _appsService.IsRequestValidOnThisTokenAsync(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = await _solutionsService.GetAsync(id);

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
                return ControllerUtilities.ProcessException<SolutionsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// An endpoint to get solutions, requires the user role
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of solutions.</returns>
        /// <response code="200">A list of solutiosn.</response>
        /// <response code="404">A message detailing any issues getting a list of solutions.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The GetSolutions endpoint requires the user to be logged in. Requires the user role. The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {},          // an object holding additional request parameters, not applicable here
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "USER")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<SudokuSolution>>> GetSolutionsAsync(
            [FromBody] Request request)
        {
            try
            {
                if (await _appsService.IsRequestValidOnThisTokenAsync(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    if (request == null) throw new ArgumentNullException(nameof(request));

                    _requestService.Update(request);

                    var result = await _solutionsService.GetSolutionsAsync(request);

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
                return ControllerUtilities.ProcessException<SolutionsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// An endpoint to solve sudoku puzzles, does not require a login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>If solvable, a solved sudoku puzzle.</returns>
        /// <response code="200">If solvable, a solved sudoku puzzle.</response>
        /// <response code="404">A message detailing any issues solving a given sudoku puzzle.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Solve endpoint does not require a logged in user. The request body parameter uses the AnnonymousCheckRequest model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {
        ///       "firstRow": integer[],   // An array of integers representing the first row of the annonymous game, unknown values are represented by 0
        ///       "secondRow": integer[],  // An array of integers representing the second row of the annonymous game, unknown values are represented by 0
        ///       "thirdRow": integer[],   // An array of integers representing the third row of the annonymous game, unknown values are represented by 0
        ///       "fourthRow": integer[],  // An array of integers representing the fourth row of the annonymous game, unknown values are represented by 0
        ///       "fifthRow": integer[],   // An array of integers representing the fifth row of the annonymous game, unknown values are represented by 0
        ///       "sixthRow": integer[],   // An array of integers representing the sixth row of the annonymous game, unknown values are represented by 0
        ///       "seventhRow": integer[], // An array of integers representing the seventhRow row of the annonymous game, unknown values are represented by 0
        ///       "eighthRow": integer[],  // An array of integers representing the eighthRow row of the annonymous game, unknown values are represented by 0
        ///       "ninthRow": integer[],   // An array of integers representing the ninthRow row of the annonymous game, unknown values are represented by 0
        ///     }     
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost, Route("Solve")]
        public async Task<ActionResult<AnnonymousGameResult>> SolveAsync(
            [FromBody] AnnonymousCheckRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var result = await _solutionsService.SolveAsync(request);

                if (result.IsSuccess)
                {
                    if (result.Payload.Count > 0)
                    {
                        result.Message = ControllerMessages.StatusCode200(result.Message);

                        return Ok(result);
                    }
                    else
                    {
                        result.Message = ControllerMessages.StatusCode200(result.Message);

                        return Ok(result);
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
                return ControllerUtilities.ProcessException<SolutionsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// An endpoint to generate sudoku puzzles, does not require a login.
        /// </summary>
        /// <returns>A sudoku puzzle.</returns>
        /// <response code="200">A sudoku puzzle.</response>
        /// <response code="404">A message detailing any issues generating a sudoku puzzle.</response>
        /// <response code="500">A description of any errors processing the request.</response>
        /// <remarks>
        /// The Generate endpoint does not require a logged in user.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost, Route("Generate")]
        public async Task<ActionResult<AnnonymousGameResult>> GenerateAsync()
        {
            try
            {
                var result = await _solutionsService.GenerateAsync();

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
                return ControllerUtilities.ProcessException<SolutionsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }

        /// <summary>
        /// An endpoint to generate solutions, requires the superuser or admin roles
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A message indicating that solutions are being created.</returns>
        /// <response code="200">A message indicating that solutions are being created.</response>
        /// <response code="404">A message detailing any issues generating solutions.</response>
        /// <response code="500">A description of any errors generating solutions.</response>
        /// <remarks>
        /// The AddSolutions endpoint requires the user to be logged in. Requires the superuser or admin roles The request body parameter uses the request model.
        /// 
        /// The request should be structured as follows:
        /// ```
        ///     {                                 
        ///       "license": string,      // the app license must be valid using the applicable regex pattern as documented in the request schema below
        ///       "requestorId": integer, // the user id for the requesting logged in user
        ///       "appId": integer,       // the app id for the app the requesting user is logged into
        ///       "paginator": paginator, // an object to control list pagination, not applicable here
        ///       "payload": {
        ///         "limit": integer, // Amount of solutions to generate, limited to 1000
        ///       }
        ///     }     
        /// ```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN")]
        [HttpPost, Route("AddSolutions")]
        public async Task<IActionResult> AddSolutionsAsync(
            [FromBody] Request request)
        {
            try
            {
                if (await _appsService.IsRequestValidOnThisTokenAsync(
                    _httpContextAccessor,
                    request.License,
                    request.AppId,
                    request.RequestorId))
                {
                    var result = _solutionsService.GenerateSolutions(request);

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
                return ControllerUtilities.ProcessException<SolutionsController>(
                    this,
                    _requestService,
                    _logger,
                    e);
            }
        }
    }
}
