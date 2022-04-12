using Microsoft.Extensions.Logging;

namespace SudokuCollective.Repos.Utilities
{
    public static class ReposUtilities
    {
        public static EventId GetRepoLogEventId()
        {
            return new EventId(300, "Repository Event");
        }

        public static EventId GetRepoErrorEventId()
        {
            return new EventId(301, "Repository Event Error");
        }
    }
}