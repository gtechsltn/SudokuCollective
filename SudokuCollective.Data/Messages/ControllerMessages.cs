namespace SudokuCollective.Data.Messages
{
    public static class ControllerMessages
    {
        public const string HelloWorld = "Hello World from Sudoku Collective!";
        public const string InvalidLicenseRequestMessage = "Status Code 400: Invalid Request on this License";
        public const string NotOwnerMessage = "Status Code 400: You are not the Owner of this App";
        public const string IdIncorrectMessage = "Status Code 400: Id is Incorrect";

        public static string StatusCode200(string serviceMessage) => string.Format("Status Code 200: {0}", serviceMessage);

        public static string StatusCode201(string serviceMessage) => string.Format("Status Code 201: {0}", serviceMessage);

        public static string StatusCode400(string serviceMessage) => string.Format("Status Code 400: {0}", serviceMessage);

        public static string StatusCode404(string serviceMessage) => string.Format("Status Code 404: {0}", serviceMessage);

        public static string StatusCode500(string serviceMessage) => string.Format("Status Code 500: {0}", serviceMessage);

        public static string Echo(string param) => string.Format("You Submitted: {0}", param);
    }
}
