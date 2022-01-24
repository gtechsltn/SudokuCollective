using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class GamesRequest : IGamesRequest
    {
        [Required]
        public int UserId { get; set; }

        public GamesRequest()
        {
            UserId = 0;
        }

        public GamesRequest(int userId)
        {
            UserId = userId;
        }
    }
}
