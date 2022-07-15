using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;

namespace SudokuCollective.Core.Interfaces.Models
{
    public interface ISettings
    {
        ICollection<IDifficulty> Difficulties { get; set;}
        ICollection<IEnumListItem> ReleaseEnvironments { get; set; }
        ICollection<IEnumListItem> SortValues { get; set; }
        ICollection<IEnumListItem> TimeFrames { get; set; }
    }
}
