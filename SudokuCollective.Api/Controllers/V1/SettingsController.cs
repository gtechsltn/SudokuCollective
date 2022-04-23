using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Core.Enums;

namespace SudokuCollective.Api.Controllers.V1
{
    /// <summary>
    /// App Controller Class
    /// </summary>
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        /// <summary>
        /// A method which returns a list of releaseEnvironments
        /// </summary>
        /// <remarks>
        /// The GetReleaseEnvironments method returns a list of release environments. Your app can be in
        /// one of the following active environments: local, staging, QA and production.  These states
        /// represent the URL routes the API will direct email links to. So for example, users will 
        /// receive an email when they sign up. This value determines which URL the email will link them 
        /// too. If the app is in production and your prodUrl is https://example-app.com then the sign up
        /// email will use the prodUrl for the email link if the release environment is set to production.
        ///
        /// When updating the app the value integer will be passed up the API to represent the apps
        /// release environment as the environment property.
        ///
        /// This method allows you to populate a dropdown list in your app if you want to control the 
        /// release environment from your app.
        ///
        /// The array returned is as follows:
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
        ///     "label": "QA",
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
        public ActionResult<List<EnumListItem>> GetReleaseEnvironments()
        {
            var releaseEnvironment = new List<string> { "releaseEnvironment" };

            var result = new List<EnumListItem>
            {
                new EnumListItem { 
                    Label = "Local", 
                    Value = (int)ReleaseEnvironment.LOCAL,
                    AppliesTo = releaseEnvironment },
                new EnumListItem { 
                    Label = "Staging", 
                    Value = (int)ReleaseEnvironment.STAGING,
                    AppliesTo = releaseEnvironment },
                new EnumListItem { 
                    Label = "QA", 
                    Value = (int)ReleaseEnvironment.QA,
                    AppliesTo = releaseEnvironment },
                new EnumListItem { 
                    Label = "Production", 
                    Value = (int)ReleaseEnvironment.PROD,
                    AppliesTo = releaseEnvironment },
            };

            return Ok(result);
        }

        /// <summary>
        /// A method which returns a list of timeFrames
        /// </summary>
        /// <remarks>
        /// The GetTimeFrames method returns a list of timeFrames. Your app uses JWT Tokens to authorize
        /// requests. As the owner of the app you can set the expiration period for the JWT Tokens, after
        /// which time the user has to reauthenticate themselves. You control the expiration period by
        /// updating two settings on your app: accessDuration and timeFrames.  AccessDuration controls the 
        /// magnitude of the expiration period and timeFrame controls the period: seconds, minutes, etc...
        ///
        /// This method allows you to populate a dropdown list in your app if you want to control the app
        /// token from within your app.
        ///
        /// The array returned is as follows:
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
        /// ]
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpGet, Route("getTimeFrames")]
        public ActionResult<List<EnumListItem>> GetTimeFrames()
        {
            var authToken = new List<string> { "authToken" };

            var result = new List<EnumListItem>
            {
                new EnumListItem { 
                    Label = "Seconds", 
                    Value = (int)TimeFrame.SECONDS,
                    AppliesTo = authToken },
                new EnumListItem { 
                    Label = "Minutes", 
                    Value = (int)TimeFrame.MINUTES,
                    AppliesTo = authToken },
                new EnumListItem { 
                    Label = "Hours", 
                    Value = (int)TimeFrame.HOURS,
                    AppliesTo = authToken },
                new EnumListItem { 
                    Label = "Days", 
                    Value = (int)TimeFrame.DAYS,
                    AppliesTo = authToken },
                new EnumListItem { 
                    Label = "Months", 
                    Value = (int)TimeFrame.MONTHS,
                    AppliesTo = authToken },
            };

            return Ok(result);
        }

        /// <summary>
        /// A method which returns a list of sortValues
        /// </summary>
        /// <remarks>
        /// The GetSortValues method returns a list of sortValues. The SudokuCollective API supports list
        /// pagination for apps, users and games. You can use respective fields for each type to paginate
        /// over.  This endpoint returns ths sort values which you can use to sort by. These values can be
        /// used to populate dropdown lists and to populate paginator items. Paginator items are as follows:
        ///
        /// ```
        ///  "paginator": {
        ///    "page": integer,                 // this param works in conjection with itemsPerPage starting with page 1
        ///    "itemsPerPage": integer          // in conjunction with page if you want items 11 through 21 page would be 2 and this would be 10
        ///    "sortBy": sortValue              // an enumeration indicating the field for sorting
        ///    "OrderByDescending": boolean     // a boolean to indicate is the order is ascending or descending
        ///    "includeCompletedGames": boolean // a boolean which only applies to game lists
        ///  },
        /// ```
        ///
        /// The integer "value" is used to populate "sortBy", thus indicating to the API which value you want
        /// to sort by.
        ///
        /// The array returned is as follows:
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
        public ActionResult<List<EnumListItem>> GetSortValues()
        {
            var all = new List<string> { "apps", "users", "games" };
            var app = new List<string> { "apps" };
            var game = new List<string> { "games" };
            var user = new List<string> { "users" };
            
            var result = new List<EnumListItem>
            {
                new EnumListItem { 
                    Label = "Id", 
                    Value = (int)SortValue.ID,
                    AppliesTo = all },
                new EnumListItem { 
                    Label = "Username", 
                    Value = (int)SortValue.USERNAME,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "First Name", 
                    Value = (int)SortValue.FIRSTNAME,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "Last Name", 
                    Value = (int)SortValue.LASTNAME,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "Full Name", 
                    Value = (int)SortValue.FULLNAME,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "Nick Name", 
                    Value = (int)SortValue.NICKNAME,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "Game Count", 
                    Value = (int)SortValue.GAMECOUNT,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "App Count", 
                    Value = (int)SortValue.APPCOUNT,
                    AppliesTo = user },
                new EnumListItem { 
                    Label = "Name", 
                    Value = (int)SortValue.NAME,
                    AppliesTo = app },
                new EnumListItem { 
                    Label = "Date Created", 
                    Value = (int)SortValue.DATECREATED,
                    AppliesTo = all },
                new EnumListItem { 
                    Label = "Date Updated", 
                    Value = (int)SortValue.DATEUPDATED,
                    AppliesTo = all },
                new EnumListItem { 
                    Label = "Difficulty Level", 
                    Value = (int)SortValue.DIFFICULTYLEVEL,
                    AppliesTo = game },
                new EnumListItem {
                    Label = "User Count",
                    Value = (int)SortValue.USERCOUNT,
                    AppliesTo = app },
                new EnumListItem { 
                    Label = "Score", 
                    Value = (int)SortValue.SCORE,
                    AppliesTo = game }
            };

            return Ok(result);
        }
    }

    /// <summary>
    /// A class to transmit enum list items to clients
    /// </summary>
    public class EnumListItem 
    {
        /// <summary>
        /// A label for dropdown items
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// A value for dropdown items
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// An array for filtering dropdown items
        /// </summary>
        public List<string> AppliesTo { get; set; }
    };
}
