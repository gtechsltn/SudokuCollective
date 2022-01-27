using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class Difficulty : IDifficulty
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }
        [JsonIgnore]
        ICollection<ISudokuMatrix> IDifficulty.Matrices
        { 
            get
            {
                return Matrices.ConvertAll(m => (ISudokuMatrix)m);
            }
            set
            {
                Matrices = value.ToList().ConvertAll(m => (SudokuMatrix)m);
            }
        }
        [JsonIgnore]
        public virtual List<SudokuMatrix> Matrices { get; set; }
        #endregion

        #region Constructors
        public Difficulty()
        {
            Id = 0;
            Name = string.Empty;
            DifficultyLevel = DifficultyLevel.NULL;
            Matrices = new List<SudokuMatrix>();
        }

        [JsonConstructor]
        public Difficulty(int id, string name,
            string displayName, DifficultyLevel difficultyLevel)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            DifficultyLevel = difficultyLevel;
            Matrices = new List<SudokuMatrix>();
        }
        #endregion
    }
}
