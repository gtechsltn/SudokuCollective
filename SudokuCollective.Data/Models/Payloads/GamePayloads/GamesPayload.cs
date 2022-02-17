using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class GamesPayload : IGamesPayload
    {
        [Required]
        public int UserId { get; set; }

        public GamesPayload()
        {
            UserId = 0;
        }

        public GamesPayload(int userId)
        {
            UserId = userId;
        }
    }
}
