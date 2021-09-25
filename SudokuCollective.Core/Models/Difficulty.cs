using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class Difficulty : IDifficulty
    {
        #region Fields
        private List<ISudokuMatrix> _matrices;
        #endregion

        #region Properties
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }
        [IgnoreDataMember]
        public virtual List<ISudokuMatrix> Matrices
        {
            get
            {
                return _matrices;
            }

            set
            {
                _matrices = value;
            }
        }
        #endregion

        #region Constructors
        public Difficulty()
        {
            _matrices = new List<ISudokuMatrix>();

            Id = 0;
            Name = string.Empty;
            DifficultyLevel = DifficultyLevel.NULL;
            Matrices = new List<ISudokuMatrix>();
        }

        [JsonConstructor]
        public Difficulty(int id, string name,
            string displayName, DifficultyLevel difficultyLevel)
        {
            _matrices = new List<ISudokuMatrix>();

            Id = id;
            Name = name;
            DisplayName = displayName;
            DifficultyLevel = difficultyLevel;
        }
        #endregion
    }
}
