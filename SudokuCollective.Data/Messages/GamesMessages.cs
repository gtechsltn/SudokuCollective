using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Api")]
namespace SudokuCollective.Data.Messages
{
    internal static class GamesMessages
    {
        internal const string GameFoundMessage = "Game Found";
        internal const string GameNotFoundMessage = "Game not Found";
        internal const string GamesFoundMessage = "Games Found";
        internal const string GamesNotFoundMessage = "Games not Found";
        internal const string GameCreatedMessage = "Game Created";
        internal const string GameNotCreatedMessage = "Game not Created";
        internal const string GameUpdatedMessage = "Game Updated";
        internal const string GameNotUpdatedMessage = "Game not Updated";
        internal const string GameDeletedMessage = "Game Deleted";
        internal const string GameNotDeletedMessage = "Game not Deleted";
        internal const string GameSolvedMessage = "Game Solved";
        internal const string GameNotSolvedMessage = "Game not Solved";
        internal const string DifficultyLevelIsRequiredMessage = "Difficulty Level is Required";
        internal const string CheckRequestNotValidMessage = "Check Request not Valid Message";
    }
}
