using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        IPaginator IRequest.Paginator
        {
            get => Paginator;
            set => Paginator = (Paginator)value;
        }
        [Required]
        public Paginator Paginator { get; set; }
        [Required]
        public JsonElement Payload { get; set; }

        public Request()
        {
            License = string.Empty;
            RequestorId = 0;
            AppId = 0;
            Paginator = new Paginator();
            Payload = new JsonElement();
        }

        public static explicit operator SudokuCollective.Logs.Models.Request(Request request)
        {
            var result = new SudokuCollective.Logs.Models.Request();

            result.License = request.License;
            result.RequestorId = request.RequestorId;
            result.AppId = request.AppId;

            return result;
        }
    }
}
