using Microsoft.Extensions.Logging;

namespace SudokuCollective.Api.Utilities
{
    /// <summary>
    /// API utility file
    /// </summary>
    public static class ApiUtilities
    {
        /// <summary>
        /// Returns an event id for controller events
        /// </summary>
        public static EventId GetControllerLogEventId()
        {
            return new EventId(100, "Controller Event");
        }

        /// <summary>
        /// Returns an event id for controller error
        /// </summary>
        public static EventId GetControllerErrorEventId()
        {
            return new EventId(101, "Controller Event Error");
        }
    }
}