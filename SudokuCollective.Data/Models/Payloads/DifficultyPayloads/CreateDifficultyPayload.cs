using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class CreateDifficultyPayload : ICreateDifficultyPayload
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        public CreateDifficultyPayload()
        {
            Name = string.Empty;
            DisplayName = string.Empty;
            DifficultyLevel = DifficultyLevel.NULL;
        }

        public CreateDifficultyPayload(
            string name, 
            string displayName, 
            int difficultyLevel)
        {
            Name = name;
            DisplayName = displayName;
            DifficultyLevel = (DifficultyLevel)difficultyLevel;
        }

        public CreateDifficultyPayload(
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
