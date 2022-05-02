using System;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Logs;
using SudokuCollective.Logs.Utilities;

[assembly:InternalsVisibleTo("SudokuCollective.Test")]
namespace SudokuCollective.Api.Utilities
{
    internal static class ControllerUtilities
    {
        internal static ObjectResult ProcessException<T>(
            ControllerBase controller,
            IRequestService requestService,
            ILogger<T> logger,
            Exception e)
        {
            var result = new Result
            {
                IsSuccess = false,
                Message = ControllerMessages.StatusCode500(e.Message)
            };

            SudokuCollectiveLogger.LogError<T>(
                logger,
                LogsUtilities.GetControllerErrorEventId(),
                result.Message,
                e,
                (SudokuCollective.Logs.Models.Request)requestService.Get());

            return controller.StatusCode((int)HttpStatusCode.InternalServerError, result);
        }

        internal static ObjectResult ProcessTokenError(ControllerBase controller)
        {
            var result = new Result
            {
                IsSuccess = false,
                Message = ControllerMessages.InvalidTokenRequestMessage
            };

            return controller.BadRequest(result);
        }
    }
}
