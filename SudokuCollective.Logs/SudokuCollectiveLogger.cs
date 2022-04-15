using Microsoft.Extensions.Logging;
using SudokuCollective.Logs.Models;

namespace SudokuCollective.Logs
{
    public static class SudokuCollectiveLogger
    {
        public static void LogInformation<T>(
            ILogger<T> logger,
            EventId eventId,
            string message,
            Request request = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException((nameof(message)));

            if (request == null)
            {
                request = new Request();
            }

            logger.LogInformation((EventId)eventId, "Message: {0}, License: {1}, AppId: {2}, RequestorId: {3}", message, request.License, request.AppId, request.RequestorId);
        }
        public static void LogWarning<T>(
            ILogger<T> logger,
            EventId eventId,
            string message,
            Request request = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException((nameof(message)));

            if (request == null)
            {
                request = new Request();
            }

            logger.LogWarning(eventId, "Message: {0}, License: {1}, AppId: {2}, RequestorId: {3}", message, request.License, request.AppId, request.RequestorId);
        }

        public static void LogError<T>(
            ILogger<T> logger, 
            EventId eventId, 
            string message,
            Exception exception,
            Request request = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException((nameof(message)));

            if (exception == null) throw new ArgumentNullException(nameof(exception));

            if (request == null)
            {
                request = new Request();
            }

            logger.LogError((EventId)eventId, exception, "Message: {0}, License: {1}, AppId: {2}, RequestorId: {3}", message, request.License, request.AppId, request.RequestorId);
        }
    }
}
