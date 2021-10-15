using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuCollective.Data.Validation
{
    public static class RegexValidators
    {
        public const string GuidRegexPattern = @"(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)";
        public const string EmailRegexPattern = @"(^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$)";
    }
}
