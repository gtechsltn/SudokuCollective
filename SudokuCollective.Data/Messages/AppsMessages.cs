using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Api")]
[assembly:InternalsVisibleTo("SudokuCollective.Test")]
namespace SudokuCollective.Data.Messages
{
    internal static class AppsMessages
    {
        internal const string AppFoundMessage = "App Found";
        internal const string AppsFoundMessage = "Apps Found";
        internal const string AppNotFoundMessage = "App not Found";
        internal const string AppsNotFoundMessage = "Apps not Found";
        internal const string AppCreatedMessage = "App Created";
        internal const string AppNotCreatedMessage = "App not Created";
        internal const string AppUpdatedMessage = "App Updated";
        internal const string AppNotUpdatedMessage = "App not Updated";
        internal const string AppResetMessage = "App Reset";
        internal const string AppNotResetMessage = "App not Reset";
        internal const string AppDeletedMessage = "App Deleted";
        internal const string AppNotDeletedMessage = "App not Deleted";
        internal const string UserAddedToAppMessage = "User Added to App";
        internal const string UserNotAddedToAppMessage = "User not Added to App";
        internal const string UserRemovedFromAppMessage = "User Removed from App";
        internal const string UserNotRemovedFromAppMessage = "User not Removed from App";
        internal const string AppActivatedMessage = "App Activated";
        internal const string AppNotActivatedMessage = "App not Activated";
        internal const string AppDeactivatedMessage = "App Deactivated";
        internal const string AppNotDeactivatedMessage = "App not Deactivated";
        internal const string UserNotSignedUpToAppMessage = "User is not Signed Up to this App";
        internal const string UserIsNotARegisteredUserOfThisAppMessage = "User is not a Registered User of this App";
        internal const string UserIsNotAnAssignedAdminMessage = "User is not an Assigned Admin";
        internal const string AdminPrivilegesActivatedMessage = "Admin Privileges Activated";
        internal const string ActivationOfAdminPrivilegesFailedMessage = "Activation of Admin Privileges Failed";
        internal const string AdminPrivilegesDeactivatedMessage = "Admin Privileges Deactivated";
        internal const string DeactivationOfAdminPrivilegesFailedMessage = "Deactivation of Admin Privileges Failed";
        internal const string AdminAppCannotBeDeletedMessage = "Admin App cannot be Deleted";
        internal const string UserIsTheAppOwnerMessage = "User is the App Owner";
        internal const string UserIsNotTheAppOwnerMessage = "User is not the App Owner";
    }
}
