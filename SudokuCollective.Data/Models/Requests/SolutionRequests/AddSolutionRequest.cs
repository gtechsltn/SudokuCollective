using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class AddSolutionRequest : IAddSolutionRequest
    {
        public int Limit { get; set; }

        public AddSolutionRequest()
        {
            Limit = 0;
        }

        public AddSolutionRequest(int limit)
        {
            Limit = limit;
        }
    }
}
