using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class GuidValidatedAttribute : RegularExpressionAttribute
    {
        public GuidValidatedAttribute() : base(RegexValidators.GuidRegexPattern)
        {
        }
    }
}
