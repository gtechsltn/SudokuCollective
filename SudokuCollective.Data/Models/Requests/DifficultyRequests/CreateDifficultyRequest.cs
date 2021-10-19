using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class CreateDifficultyRequest : ICreateDifficultyRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        public CreateDifficultyRequest()
        {
            Name = string.Empty;
            DisplayName = string.Empty;
            DifficultyLevel = DifficultyLevel.NULL;
        }

        public CreateDifficultyRequest(
            string name, 
            string displayName, 
            int difficultyLevel)
        {
            Name = name;
            DisplayName = displayName;
            DifficultyLevel = (DifficultyLevel)difficultyLevel;
        }

        public CreateDifficultyRequest(
            string name, 
            string displayName, 
            DifficultyLevel difficultyLevel)
        {
            Name = name;
            DisplayName = displayName;
            DifficultyLevel = difficultyLevel;
        }
    }
}
