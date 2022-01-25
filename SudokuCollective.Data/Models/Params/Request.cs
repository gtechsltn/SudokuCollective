using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Data.Models.Params
{
    public class Request : IRequest
    {
        [Required]
        public string License { get; set; }
        [Required]
        public int RequestorId { get; set; }
        [Required]
        public int AppId { get; set; }
        [Required]
        public IPaginator Paginator { get; set; }
        [Required]
        public object DataPacket { get; set; }

        public Request()
        {
            License = string.Empty;
            RequestorId = 0;
            AppId = 0;
            Paginator = new Paginator();
            DataPacket = null;
        }
    }
}
