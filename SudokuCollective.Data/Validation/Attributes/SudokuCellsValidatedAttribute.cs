using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class SudokuCellsValidatedAttribute : ValidationAttribute
    {
        private const string defaultError = "{0} must have {1} items.";
        private readonly int count;

        public SudokuCellsValidatedAttribute(int number) : base(defaultError)
        {
            count = number;
        }

        public override bool IsValid(object value)
        {
            return (value is List<SudokuCell> list && list.Count == count);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name, count);
        }
    }
}
