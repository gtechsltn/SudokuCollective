using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Utilities;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class SudokuCellsValidatedAttribute : ValidationAttribute
    {
        public SudokuCellsValidatedAttribute() : base() { }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            // Reject if value cannot be cast to ISudokuCell list
            if (value is not List<ISudokuCell> sudokuCells || sudokuCells.Count != 81)
            {
                return false;
            }

            foreach (var cell in sudokuCells)
            {
                if (!ModelValidator.Validate((SudokuCell)cell, out ICollection<ValidationResult> _results))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
