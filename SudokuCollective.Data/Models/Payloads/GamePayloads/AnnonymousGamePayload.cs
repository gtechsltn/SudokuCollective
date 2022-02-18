using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class AnnonymousGamePayload : IAnnonymousGamePayload
    {
        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        public AnnonymousGamePayload()
        {
            DifficultyLevel = DifficultyLevel.NULL;
        }

        public AnnonymousGamePayload(int difficultyLevel)
        {
            DifficultyLevel = (DifficultyLevel)difficultyLevel;
        }

        public AnnonymousGamePayload(DifficultyLevel difficultyLevel)
        {
            DifficultyLevel = difficultyLevel;
        }

        public static implicit operator JsonElement(AnnonymousGamePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
