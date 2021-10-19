using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class AnnonymousGameRequest : IAnnonymousGameRequest
    {
        public DifficultyLevel DifficultyLevel { get; set; }

        public AnnonymousGameRequest()
        {
            DifficultyLevel = DifficultyLevel.NULL;
        }

        public AnnonymousGameRequest(int difficultyLevel)
        {
            DifficultyLevel = (DifficultyLevel)difficultyLevel;
        }

        public AnnonymousGameRequest(DifficultyLevel difficultyLevel)
        {
            DifficultyLevel = difficultyLevel;
        }
    }
}