using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Data.Models.Params
{
    public class Request : IRequest
    {
        public string License { get; set; }
        public int RequestorId { get; set; }
        public int AppId { get; set; }
        public IPaginator Paginator { get; set; }
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
