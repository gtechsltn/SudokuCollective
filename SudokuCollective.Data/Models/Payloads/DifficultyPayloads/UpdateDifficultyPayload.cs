using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class UpdateDifficultyPayload : IUpdateDifficultyPayload
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }

        public UpdateDifficultyPayload()
        {
            Id = 0;
            Name = string.Empty;
            DisplayName = string.Empty;
        }

        public UpdateDifficultyPayload(int id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }
    }
}
