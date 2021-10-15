using System;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Data.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class GuidRegexAttribute : RegularExpressionAttribute
    {
        public GuidRegexAttribute() : base(RegexValidators.GuidRegexPattern)
        {
        }
    }
}
