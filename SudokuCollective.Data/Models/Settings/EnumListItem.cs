using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;

namespace SudokuCollective.Data.Models.Settings
{
    public class EnumListItem : IEnumListItem
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public List<string> AppliesTo { get; set; }

        public EnumListItem()
        {
            Label = string.Empty;
            Value = 0;
            AppliesTo = new List<string>();
        }
    }
}