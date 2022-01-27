using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class AnnonymousCheckRequest : IAnnonymousCheckRequest
    {
        #region Fields
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
        #endregion

        #region Properties
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
                else
                {
                    throw new ArgumentException("First row is invalid");
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
                else
                {
                    throw new ArgumentException("Second row is invalid");
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
                else
                {
                    throw new ArgumentException("Third row is invalid");
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
                else
                {
                    throw new ArgumentException("Fourth row is invalid");
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
                else
                {
                    throw new ArgumentException("Fifth row is invalid");
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
                else
                {
                    throw new ArgumentException("Sixth row is invalid");
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
                else
                {
                    throw new ArgumentException("Seventh row is invalid");
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
                else
                {
                    throw new ArgumentException("Eighth row is invalid");
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
                else
                {
                    throw new ArgumentException("Ninth row is invalid");
                }
            }
        }
        #endregion

        public AnnonymousCheckRequest() { }

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
