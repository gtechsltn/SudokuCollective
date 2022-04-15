using Microsoft.Extensions.Logging;

namespace SudokuCollective.Logs.Utilities
{
    public static class LogsUtilities
    {
        public static EventId GetControllerLogEventId()
        {
            return new EventId(100, "Controller Event");
        }

        public static EventId GetControllerWarningEventId()
        {
            return new EventId(101, "Controller Event Warning");
        }

        public static EventId GetControllerErrorEventId()
        {
            return new EventId(102, "Controller Event Error");
        }
        
        public static EventId GetServiceLogEventId()
        {
            return new EventId(200, "Service Event");
        }

        public static EventId GetServiceWarningEventId()
        {
            return new EventId(201, "Service Event Warning");
        }

        public static EventId GetServiceErrorEventId()
        {
            return new EventId(202, "Service Event Error");
        }

        public static EventId GetSMTPEventId()
        {
            return new EventId(203, "SMTP Event");
        }
        public static EventId GetRepoLogEventId()
        {
            return new EventId(300, "Repository Event");
        }

        public static EventId GetRepoWarningEventId()
        {
            return new EventId(301, "Repository Event Warning");
        }

        public static EventId GetRepoErrorEventId()
        {
            return new EventId(302, "Repository Event Error");
        }
    }
}
