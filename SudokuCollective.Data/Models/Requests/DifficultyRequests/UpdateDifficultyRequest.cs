using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateDifficultyRequest : IUpdateDifficultyRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public UpdateDifficultyRequest()
        {
            Id = 0;
            Name = string.Empty;
            DisplayName = string.Empty;
        }

        public UpdateDifficultyRequest(int id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }
    }
}
