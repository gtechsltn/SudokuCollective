using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Structs;

namespace SudokuCollective.Core.Models
{
    public class SudokuCell : ISudokuCell
    {
        #region Fields
        private int _index = 0;
        private int _column = 0;
        private int _region = 0;
        private int _row = 0;
        private int _value = 0;
        private int _displayValue = 0;
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }
        [Required, Range(1, 81, ErrorMessage = AttributeMessages.InvalidIndex)]
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value >= 1 && value <= 81)
                {
                    _index = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidIndex);
                }
            }
        }
        [Required, Range(1, 9, ErrorMessage = AttributeMessages.InvalidColumn)]
        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                if (value >= 1 && value <= 9)
                {
                    _column = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidColumn);
                }
            }
        }
        [Required, Range(1, 9, ErrorMessage = AttributeMessages.InvalidRegion)]
        public int Region
        {
            get
            {
                return _region;
            }
            set
            {
                if (value >= 1 && value <= 9)
                {
                    _region = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidRegion);
                }
            }
        }
        [Required, Range(1, 9, ErrorMessage = AttributeMessages.InvalidRow)]
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                if (value >= 1 && value <= 9)
                {
                    _row = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidRow);
                }
            }
        }
        [Required, Range(1, 9, ErrorMessage = AttributeMessages.InvalidValue)]
        public int Value
        {
            get => _value;

            set
            {
                if (value == 0)
                {
                    if (Value != 0)
                    {
                        foreach (var availableValue in AvailableValues)
                        {
                            availableValue.Available = true;
                        }
                    }
                }
                else if (value >= 1 && value <= 9)
                {
                    foreach (var availableValue in AvailableValues)
                    {
                        availableValue.Available = false;
                    }

                    if (SudokuCellEvent != null)
                    {
                        OnSuccessfulSudokuCellUpdate(
                            new SudokuCellEventArgs(
                                Index,
                                value,
                                Column,
                                Region,
                                Row
                            )
                        );
                    }
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidValue);
                }

                _value = value;
            }
        }
        [Required, Range(0, 9, ErrorMessage = AttributeMessages.InvalidDisplayedValue)]
        public int DisplayedValue
        {
            get
            {
                if (!Hidden)
                {
                    return _value;
                }
                else
                {
                    return _displayValue;
                }
            }
            set
            {
                if (value >= 0 && value <= 9)
                {
                    _displayValue = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidDisplayedValue);
                }
            }
        }
        public bool Hidden { get; set; }
        public int SudokuMatrixId { get; set; }
        [JsonIgnore]
        ISudokuMatrix ISudokuCell.SudokuMatrix
        {
            get
            {
                return SudokuMatrix;
            }
            set
            {
                SudokuMatrix = (SudokuMatrix)value;
            }
        }
        [JsonIgnore]
        public virtual SudokuMatrix SudokuMatrix { get; set; }
        [IgnoreDataMember]
        ICollection<IAvailableValue> ISudokuCell.AvailableValues
        {
            get
            {
                return AvailableValues.ConvertAll(av => (IAvailableValue)av);
            }
            set
            {
                AvailableValues = value.ToList().ConvertAll(av => (AvailableValue)av);
            }
        }
        [IgnoreDataMember]
        public List<AvailableValue> AvailableValues { get; set; }
        #endregion

        #region Constructors
        public SudokuCell()
        {
            Id = 0;
            SudokuMatrixId = 0;
            Hidden = true;
            AvailableValues = new List<AvailableValue>();

            for (var i = 1; i <= 9; i++)
            {
                AvailableValues.Add(
                        new AvailableValue
                        {
                            Value = i,
                            Available = true
                        }
                    );
            }
        }

        public SudokuCell(
            int index,
            int column,
            int region,
            int row,
            int matrixID) : this()
        {
            Index = index;
            Column = column;
            Region = region;
            Row = row;
            SudokuMatrixId = matrixID;
        }

        public SudokuCell(
            int index,
            int column,
            int region,
            int row,
            int value,
            int matrixId) : this()
        {
            Index = index;
            Column = column;
            Region = region;
            Row = row;
            SudokuMatrixId = matrixId;
            Value = value;

            if (Value != 0)
            {
                foreach (var availableValue in AvailableValues)
                {
                    availableValue.Available = false;
                }
            }
        }

        [JsonConstructor]
        public SudokuCell(
            int id,
            int index,
            int column,
            int region,
            int row,
            int value,
            int displayedValue,
            bool hidden,
            int sudokuMatrixId)
        {
            Id = id;
            Index = index;
            Column = column;
            Region = region;
            Row = row;
            _value = value;
            DisplayedValue = displayedValue;
            Hidden = hidden;
            SudokuMatrixId = sudokuMatrixId;
            AvailableValues = new List<AvailableValue>();

            var availability = true;

            if (value > 0)
            {
                availability = false;
            }

            for (var i = 1; i <= 9; i++)
            {
                AvailableValues.Add(
                        new AvailableValue
                        {
                            Value = i,
                            Available = availability
                        }
                    );
            }
        }
        #endregion

        #region Methods
        public int ToInt32() => DisplayedValue;

        public override string ToString() => DisplayedValue.ToString();

        public void UpdateAvailableValues(int i)
        {
            if (AvailableValues.Any(a => a.Value == i && a.Available))
            {
                var availableValue = AvailableValues
                    .Where(a => a.Value == i)
                    .FirstOrDefault();

                availableValue.Available = false;

                if (AvailableValues.Where(a => a.Available).ToList().Count == 1)
                {
                    var finalAvailableValue = AvailableValues
                        .Where(a => a.Available)
                        .FirstOrDefault();

                    Value = finalAvailableValue.Value;

                    finalAvailableValue.Available = false;
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<SudokuCellEventArgs> SudokuCellEvent;

        public virtual void OnSuccessfulSudokuCellUpdate(SudokuCellEventArgs e)
        {
            SudokuCellEvent.Invoke(this, e);
        }
        #endregion
    }
}
