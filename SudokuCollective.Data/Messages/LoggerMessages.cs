using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Repos")]
namespace SudokuCollective.Data.Messages
{
    internal static class LoggerMessages
    {
        internal const string ErrorThrownMessage = "Following error thrown: {0}";
        internal const string TheLicenseIsNotValidOnThisRequest = "The License is not valid on this Request";
        internal const string TheUserOrAppIsNotValidForThisJWTToken = "The user or app is not valid for this JWT Token";
        internal const string HttpContextAccessorIsNull = "HttpContextAccessor is null";
        internal const string AppIsNotActive = "{0} is not Active";
    }
}
