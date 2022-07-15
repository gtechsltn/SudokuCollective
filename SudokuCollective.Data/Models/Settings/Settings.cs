using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Utilities;

namespace SudokuCollective.Data.Models.Settings
{
    public class Settings : ISettings
    {
        [JsonIgnore]
        ICollection<IDifficulty> ISettings.Difficulties
        {
            get => Difficulties.ConvertAll(d => (IDifficulty)d);
            set => Difficulties = value.ToList().ConvertAll(d => (Difficulty)d);
        }
        [JsonPropertyName("difficulties"), JsonConverter(typeof(IDomainEntityListConverter<List<Difficulty>>))]
        public List<Difficulty> Difficulties { get; set; }
        [JsonIgnore]
        ICollection<IEnumListItem> ISettings.ReleaseEnvironments
        {
            get => ReleaseEnvironments.ConvertAll(r => (IEnumListItem)r);
            set => ReleaseEnvironments = value.ToList().ConvertAll(r => (EnumListItem)r);
        }
        [JsonPropertyName("releaseEnvironments")]
        public List<EnumListItem> ReleaseEnvironments { get; set; }
        [JsonIgnore]
        ICollection<IEnumListItem> ISettings.SortValues
        {
            get => SortValues.ConvertAll(r => (IEnumListItem)r);
            set => SortValues = value.ToList().ConvertAll(r => (EnumListItem)r);
        }
        [JsonPropertyName("sortValues")]
        public List<EnumListItem> SortValues { get; set; }
        [JsonIgnore]
        ICollection<IEnumListItem> ISettings.TimeFrames
        {
            get => TimeFrames.ConvertAll(t => (IEnumListItem)t);
            set => TimeFrames = value.ToList().ConvertAll(t => (EnumListItem)t);
        }
        [JsonPropertyName("timeFrames")]
        public List<EnumListItem> TimeFrames { get; set; }

        public Settings()
        {
            Difficulties = new List<Difficulty>();
            ReleaseEnvironments = new List<EnumListItem>();
            SortValues = new List<EnumListItem>();
            TimeFrames = new List<EnumListItem>();
        }
    }
}
