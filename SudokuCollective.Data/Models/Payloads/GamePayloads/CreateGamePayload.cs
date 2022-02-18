using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class CreateGamePayload : ICreateGamePayload
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DifficultyId { get; set; }

        public CreateGamePayload()
        {
            UserId = 0;
            DifficultyId = 0;
        }

        public CreateGamePayload(int userId, int difficultyId)
        {
            UserId = userId;
            DifficultyId = difficultyId;
        }

        public static implicit operator JsonElement(CreateGamePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
