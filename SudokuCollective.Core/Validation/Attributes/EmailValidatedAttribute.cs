using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class EmailValidatedAttribute : RegularExpressionAttribute
    {
        public EmailValidatedAttribute() : base(RegexValidators.EmailRegexPattern)
        {
        }
    }
}
