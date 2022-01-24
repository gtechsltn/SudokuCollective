using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class CreateGameRequest : ICreateGameRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DifficultyId { get; set; }

        public CreateGameRequest()
        {
            UserId = 0;
            DifficultyId = 0;
        }

        public CreateGameRequest(int userId, int difficultyId)
        {
            UserId = userId;
            DifficultyId = difficultyId;
        }

    }
}
