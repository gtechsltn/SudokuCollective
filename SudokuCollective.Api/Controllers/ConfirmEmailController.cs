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
        /// A method to process requests on the confirm email controller.
        /// </summary>
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