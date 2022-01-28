namespace SudokuCollective.Core.Messages
{
    public static class AttributeMessages
    {
        public const string InvalidUserName = "User name must be at least 4 characters and can contain alphanumeric characters and special characters of [! @ # $ % ^ & * + = ? - _ . ,]";
        public const string InvalidEmail = "Email must be in a valid format";
        public const string InvalidLicense = "License must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters";
        public const string InvalidToken = "Token must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters";
        public const string InvalidOldEmail = "Old email must be in a valid format";
        public const string InvalidNewEmail = "New email must be in a valid format";
        public const string InvalidIndex = "Sudoku Cell index must be between 1 and 81";
        public const string InvalidColumn = "Sudoku cell column must be between 1 and 9";
        public const string InvalidRegion = "Sudoku cell region must be between 1 and 9";
        public const string InvalidRow = "Sudoku cell row must be between 1 and 9";
        public const string InvalidValue = "Sudoku cell value must be between 1 and 9";
        public const string InvalidDisplayedValue = "Sudoku cell displayed value must be between 0 and 9";
        public const string InvalidSudokuCells = "The list of sudoku values is not valid";
        public const string InvalidPassword = "Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of [! @ # $ % ^ & * + = ? - _ . ,]";
        public const string InvalidFirstRow = "First row is invalid";
        public const string InvalidSecondRow = "Second row is invalid";
        public const string InvalidThirdRow = "Third row is invalid";
        public const string InvalidFourthRow = "Fourth row is invalid";
        public const string InvalidFifthRow = "FIfth row is invalid";
        public const string InvalidSixthRow = "Sixth row is invalid";
        public const string InvalidSeventhRow = "Seventh row is invalid";
        public const string InvalidEighthRow = "Eighth row is invalid";
        public const string InvalidNinthRow = "Ninth row is invalid";
    }
}
