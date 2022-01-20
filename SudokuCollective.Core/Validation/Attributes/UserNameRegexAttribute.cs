using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class UserNameRegexAttribute : RegularExpressionAttribute
    {
        public UserNameRegexAttribute() : base(RegexValidators.UserNameRegexPattern)
        {
        }
    }
}
