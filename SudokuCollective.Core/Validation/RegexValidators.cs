using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Test")]
namespace SudokuCollective.Core.Validation
{
    internal static class RegexValidators
    {
        // Email must be in a valid format
        internal const string EmailRegexPattern = @"(^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$)";
        // Guid must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters
        internal const string GuidRegexPattern = @"(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)";
        /* Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 
         * special character of [! @ # $ % ^ & * + = ? - _ . ,] */
        internal const string PasswordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*+=?-_.,]).{4,20}$";
        /* User name must be at least 4 characters and can contain alphanumeric characters and special characters of
         * [! @ # $ % ^ & * + = ? - _ . ,] */
        internal const string UserNameRegexPattern = @"^[a-zA-Z0-9!@#$%^&*+=<>?-_.,].{3,}$";
    }
}
