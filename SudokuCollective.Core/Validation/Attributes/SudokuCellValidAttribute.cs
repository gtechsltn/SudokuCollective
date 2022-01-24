using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Utilities;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class SudokuCellValidAttribute : ValidationAttribute
    {
        public SudokuCellValidAttribute() : base() { }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var sudokuCells = value as List<SudokuCell>;

            // Reject if value cannot be cast to int list
            if (sudokuCells == null || sudokuCells.Count != 81)
            {
                return false;
            }

            foreach (var cell in sudokuCells)
            {
                if (!ModelValidator.Validate(cell, out ICollection<ValidationResult> _results))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
