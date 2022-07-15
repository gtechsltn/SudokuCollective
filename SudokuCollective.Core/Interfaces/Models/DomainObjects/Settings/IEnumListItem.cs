using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings
{
    public interface IEnumListItem
    {
        string Label { get; set; }
        int Value { get; set; }
        List<string> AppliesTo { get; set; }
    }
}
