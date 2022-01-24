using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class AnnonymousCheckRequest : IAnnonymousCheckRequest
    {
        private List<int> _firstRow = new();
        private List<int> _secondRow = new();
        private List<int> _thirdRow = new();
        private List<int> _fourthRow = new();
        private List<int> _fifthRow = new();
        private List<int> _sixthRow = new();
        private List<int> _seventhRow = new();
        private List<int> _eighthRow = new();
        private List<int> _ninthRow = new();
        private readonly RowValidatedAttribute _rowValidator = new();

        [Required, RowValidated]
        public List<int> FirstRow
        {
            get
            {
                return _firstRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _firstRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> SecondRow
        {
            get
            {
                return _secondRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _secondRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> ThirdRow
        {
            get
            {
                return _thirdRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _thirdRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> FourthRow
        {
            get
            {
                return _fourthRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _fourthRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> FifthRow
        {
            get
            {
                return _fifthRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _fifthRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> SixthRow
        {
            get
            {
                return _sixthRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _sixthRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> SeventhRow
        {
            get
            {
                return _seventhRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _seventhRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> EighthRow
        {
            get
            {
                return _eighthRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _eighthRow = value;
                }
            }
        }
        [Required, RowValidated]
        public List<int> NinthRow
        {
            get
            {
                return _ninthRow;
            }
            set
            {
                if (value != null && _rowValidator.IsValid(value))
                {
                    _ninthRow = value;
                }
            }
        }

        public AnnonymousCheckRequest()
        {
            FirstRow = new List<int>();
            SecondRow = new List<int>();
            ThirdRow = new List<int>();
            FourthRow = new List<int>();
            FifthRow = new List<int>();
            SixthRow = new List<int>();
            SeventhRow = new List<int>();
            EighthRow = new List<int>();
            NinthRow = new List<int>();
        }

        public AnnonymousCheckRequest(
            int[] firstRow, 
            int[] secondRow, 
            int[] thirdRow, 
            int[] fourthRow, 
            int[] fifthRow, 
            int[] sixthRow, 
            int[] seventhRow, 
            int[] eighthRow, 
            int[] ninthRow)
        {
            FirstRow = firstRow.ToList();
            SecondRow = secondRow.ToList();
            ThirdRow = thirdRow.ToList();
            FourthRow = fourthRow.ToList();
            FifthRow = fifthRow.ToList();
            SixthRow = sixthRow.ToList();
            SeventhRow = seventhRow.ToList();
            EighthRow = eighthRow.ToList();
            NinthRow = ninthRow.ToList();
        }

        public AnnonymousCheckRequest(
            List<int> firstRow, 
            List<int> secondRow, 
            List<int> thirdRow, 
            List<int> fourthRow, 
            List<int> fifthRow, 
            List<int> sixthRow, 
            List<int> seventhRow, 
            List<int> eighthRow, 
            List<int> ninthRow)
        {
            FirstRow = firstRow;
            SecondRow = secondRow;
            ThirdRow = thirdRow;
            FourthRow = fourthRow;
            FifthRow = fifthRow;
            SixthRow = sixthRow;
            SeventhRow = seventhRow;
            EighthRow = eighthRow;
            NinthRow = ninthRow;
        }
    }
}
