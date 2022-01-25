using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class UserNameValidatedAttribute : RegularExpressionAttribute
    {
        public UserNameValidatedAttribute() : base(RegexValidators.UserNameRegexPattern)
        {
        }
    }
}
