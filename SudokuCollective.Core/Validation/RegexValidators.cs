namespace SudokuCollective.Core.Validation
{
    public static class RegexValidators
    {
        // Validates emails
        public const string EmailRegexPattern = @"(^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$)";
        // Must be at least 4 characters and accepts alphanumeric and special characters
        // Verifies that a string meets the guid pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with alpha numeric characters
        public const string GuidRegexPattern = @"(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)";
        /* Between 4 and up to 20 characters with at least 1 capital letter, one lower case letter and one 
         * special character of [!,@,#,$,%,^,&,*,+,=,?,-,_,.,,] */
        public const string PasswordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*+=?-_.,]).{4,20}$";
        // Accepts a alphanumeric and special characters of [!,@,#,$,%,^,&,*,+,=,?,-,_,.,,] with a minimum of 4 characters
        public const string UserNameRegexPattern = @"^[a-zA-Z0-9!@#$%^&*+=<>?-_.,].{3,}$";
    }
}
