using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class SudokuSolution : ISudokuSolution
    {
        #region Properites
        [Required]
        public int Id { get; set; }
        [Required]
        public virtual List<int> SolutionList { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateSolved { get; set; }
        public List<int> FirstRow { get => GetValues(0, 9); }
        public List<int> SecondRow { get => GetValues(9, 9); }
        public List<int> ThirdRow { get => GetValues(18, 9); }
        public List<int> FourthRow { get => GetValues(27, 9); }
        public List<int> FifthRow { get => GetValues(36, 9); }
        public List<int> SixthRow { get => GetValues(45, 9); }
        public List<int> SeventhRow { get => GetValues(54, 9); }
        public List<int> EighthRow { get => GetValues(63, 9); }
        public List<int> NinthRow { get => GetValues(72, 9); }
        [IgnoreDataMember]
        IGame ISudokuSolution.Game
        {
            get
            {
                return Game;
            }
            set
            {
                Game = (Game)value;
            }
        }
        [IgnoreDataMember]
        public virtual Game Game { get; set; }
        #endregion

        #region Constructors
        public SudokuSolution()
        {
            var createdDate = DateTime.UtcNow;

            Id = 0;
            DateCreated = createdDate;
            DateSolved = DateTime.MinValue;
            SolutionList = new List<int>();

            for (var i = 0; i < 81; i++)
            {
                SolutionList.Add(0);
            }
        }

        public SudokuSolution(List<int> intList) : this()
        {
            var solvedDate = DateTime.UtcNow;
            DateSolved = solvedDate;

            SolutionList = new List<int>();
            SolutionList = intList;
        }

        [JsonConstructor]
        public SudokuSolution(
            int id,
            List<int> intList,
            DateTime dateCreated, 
            DateTime dateSolved)
        {
            Id = id;
            SolutionList = intList;
            DateCreated = dateCreated;
            DateSolved = dateSolved;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (var solutionListInt in SolutionList)
            {
                result.Append(solutionListInt);
            }

            return result.ToString();
        }

        private List<int> GetValues(int skipValue, int takeValue)
        {
            return SolutionList.Skip(skipValue).Take(takeValue).ToList();
        }
        #endregion
    }
}
