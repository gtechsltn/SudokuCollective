using System.Text.Json;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class AddSolutionPayload : IAddSolutionPayload
    {
        public int Limit { get; set; }

        public AddSolutionPayload()
        {
            Limit = 0;
        }

        public AddSolutionPayload(int limit)
        {
            Limit = limit;
        }

        public static implicit operator JsonElement(AddSolutionPayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
