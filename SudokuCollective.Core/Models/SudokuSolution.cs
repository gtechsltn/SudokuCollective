using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public virtual ICollection<int> SolutionList { get; set; }
        public DateTime DateCreated { get; set; }
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
        public virtual IGame Game { get; set; }
        #endregion

        #region Constructors
        public SudokuSolution()
        {
            var createdDate = DateTime.UtcNow;

            Id = 0;
            SolutionList = new List<int>();
            DateCreated = createdDate;
            DateSolved = DateTime.MinValue;
        }

        public SudokuSolution(List<int> intList) : this()
        {
            var solvedDate = DateTime.UtcNow;

            SolutionList = intList;
            DateSolved = solvedDate;
        }

        [JsonConstructor]
        public SudokuSolution(int id, List<int> intList,
            DateTime dateCreated, DateTime dateSolved)
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
