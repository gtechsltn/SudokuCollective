using System.Threading.Tasks.Dataflow;

namespace SudokuCollective.Data.Messages
{
    public static class SolutionsMessages
    {
        public const string SolutionFoundMessage = "Solution Found";
        public const string SolutionNotFoundMessage = "Solution not Found";
        public const string SolutionsFoundMessage = "Solutions Found";
        public const string SolutionsNotFoundMessage = "Solutions not Found";
        public const string SolutionCreatedMessage = "Solution Created";
        public const string SolutionNotCreatedMessage = "Solution not Created";
        public const string SolutionUpdatedMessage = "Solution Updated";
        public const string SolutionNotUpdatedMessage = "Solution not Updated";
        public const string SolutionsAddedMessage = "Solutions Added";
        public const string SolutionsNotAddedMessage = "Solutions not Added";
        public const string SudokuSolutionFoundMessage = "Sudoku Solution Found";
        public const string SudokuSolutionNotFoundMessage = "Sudoku Solution not Found";
        public const string SolutionGeneratedMessage = "Solution Generated";
        public const string SolutionNotGeneratedMessage = "Solution not Generated";
    }
}
