using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Api")]
namespace SudokuCollective.Data.Messages
{
    internal static class ControllerMessages
    {
        internal const string HelloWorld = "Hello World from Sudoku Collective!";
        internal const string InvalidTokenRequestMessage = "Status Code 400: Invalid Request on this Token";
        internal const string NotOwnerMessage = "Status Code 400: You are not the Owner of this App";
        internal const string IdIncorrectMessage = "Status Code 400: Id is Incorrect";
        internal const string IdCannotBeZeroMessage = "Id cannot be zero";
        internal const string UserIdCannotBeZeroMessage = "User id cannot be zero";

        internal static string StatusCode200(string serviceMessage) => string.Format("Status Code 200: {0}", serviceMessage);

        internal static string StatusCode201(string serviceMessage) => string.Format("Status Code 201: {0}", serviceMessage);

        internal static string StatusCode400(string serviceMessage) => string.Format("Status Code 400: {0}", serviceMessage);

        internal static string StatusCode404(string serviceMessage) => string.Format("Status Code 404: {0}", serviceMessage);

        internal static string StatusCode500(string serviceMessage) => string.Format("Status Code 500: {0}", serviceMessage);

        internal static string Echo(string param) => string.Format("You Submitted: {0}", param);
    }
}
