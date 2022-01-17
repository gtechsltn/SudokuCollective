using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Structs;

namespace SudokuCollective.Core.Models
{
    public class SudokuCell : ISudokuCell
    {
        #region Fields
        private int _value;
        private int _displayValue;
        #endregion

        #region Properties
        public int Id { get; set; }
        public int Index { get; set; }
        public int Column { get; set; }
        public int Region { get; set; }
        public int Row { get; set; }
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
                else
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

                _value = value;
            }
        }
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
                _displayValue = value;
            }
        }
        public bool Hidden { get; set; }
        public int SudokuMatrixId { get; set; }
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [JsonIgnore]
        public List<AvailableValue> AvailableValues { get; set; }
        #endregion

        #region Constructors
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
            Value = 0;
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

        public SudokuCell()
        {
            Id = 0;
            Index = 0;
            Column = 0;
            Region = 0;
            Row = 0;
            Value = 0;
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
