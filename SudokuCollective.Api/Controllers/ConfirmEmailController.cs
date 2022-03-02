using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SudokuCollective.Api.Models;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Api.Controllers
{
    /// <summary>
    /// Confirm Email Controller
    /// </summary>
    [Route("[controller]")]
    [Controller]
    public class ConfirmEmailController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IWebHostEnvironment _hostEnvironment;

        /// <summary>
        /// A class constructor which serves as a DI hook.
        /// </summary>
        public ConfirmEmailController(
            IUsersService usersServ,
            IWebHostEnvironment environment)
        {
            _usersService = usersServ;
            _hostEnvironment = environment;
        }

        /// <summary>
        /// A default method to process confirm email requests.
        /// </summary>
        /// <remarks>
        /// This is a default method to handle email confirmations.  It is strongly
        /// recommended that you implement a custom email confirmation action to 
        /// handle such requests, this method is simply a placeholder to handle
        /// such requests until you've implemented a custom action.  In order to 
        /// implement such a request you have to create it within your app (the 
        /// details of which are dependent upon your apps particular framework) 
        /// and then enable it by setting the following app properties:
        ///
        /// ```DisableCustomUrls``` = ```false```
        ///
        /// ```UseCustomEmailConfirmationAction``` = ```true```
        ///
        /// ```CustomEmailConfirmationAction``` = the custom action you've created
        ///
        /// So if the url for your app is https://yourapp and the custom action is
        /// ```confirmEmail``` then your users will be directed to the following:
        ///
        /// ```https://yourapp/confirmEmail/{token}```
        ///
        /// Please note the url is dependent on the release environment, so if your
        /// release environment is set to local the requests will be directed to your
        /// local url and if set to staging the requests will be directed to your
        /// staging url, etc.
        ///
        /// Until such time as the above conditions are met such requests will continue
        /// to be directed to this default page.
        ///
        /// The token will be provided by the api and will be sent to the user in the 
        /// confirmation email.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{token}")]
        public async Task<IActionResult> Index(string token)
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
                var confirmEmailModel = new ConfirmEmail
                {
                    UserName = ((ConfirmEmailResult)result.Payload[0]).UserName,
                    AppTitle = ((ConfirmEmailResult)result.Payload[0]).AppTitle,
                    Url = ((ConfirmEmailResult)result.Payload[0]).Url,
                    IsUpdate = ((ConfirmEmailResult)result.Payload[0]).IsUpdate != null && 
                        (bool)((ConfirmEmailResult)result.Payload[0]).IsUpdate,
                    NewEmailAddressConfirmed = ((ConfirmEmailResult)result.Payload[0]).NewEmailAddressConfirmed != null && 
                        (bool)((ConfirmEmailResult)result.Payload[0]).NewEmailAddressConfirmed,
                    IsSuccess = result.IsSuccess
                };

                return View(confirmEmailModel);
            }
            else
            {
                var confirmEmailModel = new ConfirmEmail
                {
                    IsSuccess = result.IsSuccess
                };

                return View(confirmEmailModel);
            }
        } 
    }
}