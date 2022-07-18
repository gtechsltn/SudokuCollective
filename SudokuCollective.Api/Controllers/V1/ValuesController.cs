using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// Values Controller Class
    /// </summary>
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IValuesService _valuesService;

        /// <summary>
        /// Values Controller Constructor
        /// </summary>
        /// <param name="valuesService"></param>
        public ValuesController(IValuesService valuesService)
        {
            _valuesService = valuesService;
        }

        /// <summary>
        /// An endpoint used to return all menu items you will use you in your settings, does not require a login.
        /// </summary>
        /// <remarks>
        /// The Get endpoint returns all menu items you will use in your various drop down settings.  The results
        /// are as follows:
        /// 
        /// ```
        /// {
        ///   "isSuccess": true,                                // An indicator if the request was successful
        ///   "isFromCache": false,                             // An indicator if the payload was obtained from the cache
        ///   "message": "Status Code 200: Values Retrieved", // A brief description of the result
        ///   "payload": [
        ///    {
        ///     difficulties: [],        // an array of all difficulties that can be applied to a game
        ///     releaseEnvironments: [], // an array of all release environment states you app can be in
        ///     sortValues, [],          // an array of all possible sort values that apps, users, and games can apply
        ///     timeFrames, [],          // an array of all intervals that can be applied to your apps auth token
        ///    }
        ///   ]
        /// }
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Result>> GetAsync()
        {
            var result = await _valuesService.GetAsync();

            if (result.IsSuccess)
            {
                result.Message = ControllerMessages.StatusCode200(result.Message);

                return Ok(result);
            }
            else
            {
                result.Message = ControllerMessages.StatusCode400(result.Message);

                return NotFound(result);
            }
        }

        /// <summary>
        /// An endpoint which returns a list of releaseEnvironments, does not require a login.
        /// </summary>
        /// <remarks>
        /// The GetReleaseEnvironments endpoint returns a list of release environments. Your app can be in
        /// one of the following active environments: local, staging, QA and production.  These states
        /// represent the URL routes the API will direct email links to. So for example, users will 
        /// receive an email when they sign up. This value determines which URL the email will link them 
        /// too. If the app is in production and your prodUrl is https://example-app.com then the sign up
        /// email will use the prodUrl for the email link if the release environment is set to production.
        ///
        /// When updating the app the value integer will be passed up the API to represent the apps
        /// release environment as the environment property.
        ///
        /// This endpoint allows you to populate a dropdown list in your app if you want to control the 
        /// release environment from your app.
        ///
        /// The uses the standard result object and the payload is as follows:
        ///
        /// ```
        /// [
        ///   {
        ///     "label": "Local",
        ///     "value": 1,
        ///     "appliesTo": [ 
        ///       "releaseEnvironment"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Staging",
        ///     "value": 2,
        ///     "appliesTo": [ 
        ///       "releaseEnvironment"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Quality Assurance",
        ///     "value": 3,
        ///     "appliesTo": [ 
        ///       "releaseEnvironment"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Production",
        ///     "value": 4,
        ///     "appliesTo": [ 
        ///       "releaseEnvironment"
        ///     ]
        ///   },
        /// ]
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpGet, Route("getReleaseEnvironments")]
        public ActionResult<Result> GetReleaseEnvironments()
        {
            var result = _valuesService.GetReleaseEnvironments();

            if (result.IsSuccess)
            {
                result.Message = ControllerMessages.StatusCode200(result.Message);

                return Ok(result);
            }
            else
            {
                result.Message = ControllerMessages.StatusCode400(result.Message);

                return NotFound(result);
            }
        }

        /// <summary>
        /// An endpoint which returns a list of sortValues, does not require a login.
        /// </summary>
        /// <remarks>
        /// The GetSortValues endpoint returns a list of sortValues. The SudokuCollective API supports list
        /// pagination for apps, users and games. You can use respective fields for each type to paginate
        /// over.  This endpoint returns ths sort values which you can use to sort by. These values can be
        /// used to populate dropdown lists and to populate paginator items. Paginator items are as follows:
        ///
        /// ```
        ///  "paginator": {
        ///    "page": integer,                 // this param works in conjection with itemsPerPage starting with page 1
        ///    "itemsPerPage": integer          // in conjunction with page if you want items 11 through 20 page would be 2 and this would be 10
        ///    "sortBy": sortValue              // an enumeration indicating the field for sorting
        ///    "OrderByDescending": boolean     // a boolean to indicate is the order is ascending or descending
        ///    "includeCompletedGames": boolean // a boolean which only applies to game lists
        ///  },
        /// ```
        ///
        /// The integer "value" is used to populate "sortBy", thus indicating to the API which value you want
        /// to sort by.
        ///
        /// The uses the standard result object and the payload is as follows:
        /// ```
        /// [
        ///   {
        ///     "label": "Id",
        ///     "value": 1,
        ///     "appliesTo": [
        ///       "apps",
        ///       "users",
        ///       "games"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Username",
        ///     "value": 2,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "First Name",
        ///     "value": 3,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Last Name",
        ///     "value": 4,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Full Name",
        ///     "value": 5,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Nick Name",
        ///     "value": 6,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Game Count",
        ///     "value": 7,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "App Count",
        ///     "value": 8,
        ///     "appliesTo": [
        ///       "users"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Name",
        ///     "value": 9,
        ///     "appliesTo": [
        ///       "apps"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Date Created",
        ///     "value": 10,
        ///     "appliesTo": [
        ///       "apps",
        ///       "users",
        ///       "games"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Date Updated",
        ///     "value": 11,
        ///     "appliesTo": [
        ///       "apps",
        ///       "users",
        ///       "games"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Difficulty Level",
        ///     "value": 12,
        ///     "appliesTo": [
        ///       "games"
        ///     ]
        ///   },
        ///   {
        ///     "label": "User Count",
        ///     "value": 13,
        ///     "appliesTo": [
        ///       "apps"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Score",
        ///     "value": 14,
        ///     "appliesTo": [
        ///       "games"
        ///     ]
        ///   }
        /// ]
        ///```
        /// </remarks>
        [Authorize(Roles = "SUPERUSER, ADMIN, USER")]
        [HttpGet, Route("getSortValues")]
        public ActionResult<Result> GetSortValues()
        {
            var result = _valuesService.GetSortValues();

            if (result.IsSuccess)
            {
                result.Message = ControllerMessages.StatusCode200(result.Message);

                return Ok(result);
            }
            else
            {
                result.Message = ControllerMessages.StatusCode400(result.Message);

                return NotFound(result);
            }
        }

        /// <summary>
        /// An endpoint which returns a list of timeFrames, does not require a login.
        /// </summary>
        /// <remarks>
        /// The GetTimeFrames endpoint returns a list of timeFrames. Your app uses JWT Tokens to authorize
        /// requests. As the owner of the app you can set the expiration period for the JWT Tokens, after
        /// which time the user has to reauthenticate themselves. You control the expiration period by
        /// updating two settings on your app: accessDuration and timeFrames.  AccessDuration controls the 
        /// magnitude of the expiration period and timeFrame controls the period: seconds, minutes, etc...
        /// 
        /// Please note if timeFrame is set to "Years" then accessDuration is limited to 5.
        ///
        /// This endpoint allows you to populate a dropdown list in your app if you want to control the app
        /// token from within your app.
        ///
        /// The uses the standard result object and the payload is as follows:
        ///
        /// ```
        /// [
        ///   {
        ///     "label": "Seconds",
        ///     "value": 1,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Minutes",
        ///     "value": 2,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Hours",
        ///     "value": 3,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Days",
        ///     "value": 4,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Months",
        ///     "value": 5,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        ///   {
        ///     "label": "Years",
        ///     "value": 6,
        ///     "appliesTo": [ 
        ///       "authToken"
        ///     ]
        ///   },
        /// ]
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpGet, Route("getTimeFrames")]
        public ActionResult<Result> GetTimeFrames()
        {
            var result = _valuesService.GetTimeFrames();

            if (result.IsSuccess)
            {
                result.Message = ControllerMessages.StatusCode200(result.Message);

                return Ok(result);
            }
            else
            {
                result.Message = ControllerMessages.StatusCode400(result.Message);

                return NotFound(result);
            }
        }
    }
}
