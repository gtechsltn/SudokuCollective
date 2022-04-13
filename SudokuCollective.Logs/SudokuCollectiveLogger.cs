using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SudokuCollective.Logs
{
    public static class SudokuCollectiveLogger
    {
        public static void LogInformation<T>(IHttpContextAccessor httpContextAccessor, ILogger<T> logger, EventId eventId, string message)
        {
            var data = httpContextAccessor.HttpContext.Request.Form;

            logger.LogInformation(eventId, message);
        }
    }
}
