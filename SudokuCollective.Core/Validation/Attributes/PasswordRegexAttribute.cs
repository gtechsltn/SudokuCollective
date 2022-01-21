using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    sealed public class PasswordRegexAttribute : RegularExpressionAttribute
    {
        public PasswordRegexAttribute() : base(RegexValidators.PasswordRegexPattern)
        {
        }
    }
}
