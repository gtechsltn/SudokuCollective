using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class CreateGameRequest : ICreateGameRequest
    {
        public int UserId { get; set; }
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
