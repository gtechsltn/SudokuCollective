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
    }
}
