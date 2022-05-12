using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Test")]
namespace SudokuCollective.Data.Messages
{
    internal static class SolutionsMessages
    {
        internal const string SolutionFoundMessage = "Solution Found";
        internal const string SolutionNotFoundMessage = "Solution not Found";
        internal const string SolutionsFoundMessage = "Solutions Found";
        internal const string SolutionsNotFoundMessage = "Solutions not Found";
        internal const string SolutionCreatedMessage = "Solution Created";
        internal const string SolutionNotCreatedMessage = "Solution not Created";
        internal const string SolutionUpdatedMessage = "Solution Updated";
        internal const string SolutionNotUpdatedMessage = "Solution not Updated";
        internal const string SolutionsAddedMessage = "Solutions Added";
        internal const string SolutionsNotAddedMessage = "Solutions not Added";
        internal const string SudokuSolutionFoundMessage = "Sudoku Solution Found";
        internal const string SudokuSolutionNotFoundMessage = "Sudoku Solution not Found";
        internal const string SolutionGeneratedMessage = "Solution Generated";
        internal const string SolutionNotGeneratedMessage = "Solution not Generated";
        internal static string LimitExceedsSolutionsLimitMessage(string limit) => string.Format("The Amount of Solutions Requested, {0}, Exceeds the Service's 1,000 Limit", limit);
    }
}
