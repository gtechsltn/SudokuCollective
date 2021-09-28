using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Data.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class RowValidatedAttribute : ValidationAttribute
    {
        private const string defaultError = "{0} is invalid.";

        public RowValidatedAttribute() : base(defaultError)
        {

        }

        public override bool IsValid(object value)
        {
            var instanceArray = value as List<int>;

            var possibleIntList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var containsDuplicates = false;

            foreach (var i in instanceArray)
            {
                if (i != 0)
                {
                    if (possibleIntList.Contains(i))
                    {
                        possibleIntList.Remove(i);
                    }
                    else
                    {
                        containsDuplicates = true;
                    }
                }
            }

            if (!containsDuplicates && instanceArray.Count == 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            name = name.Replace("Row", " Row");
            return string.Format(this.ErrorMessageString, name);
        }
    }
}
