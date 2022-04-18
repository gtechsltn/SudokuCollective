using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SudokuCollective.Test")]
namespace SudokuCollective.Data.Messages
{
    internal class UsersMessages
    {
        internal const string UserFoundMessage = "User Found";
        internal const string UsersFoundMessage = "Users Found";
        internal const string UserNotFoundMessage = "User not Found";
        internal const string UsersNotFoundMessage = "Users not Found";
        internal const string UserCreatedMessage = "User Created";
        internal const string UserNotCreatedMessage = "User not Created";
        internal const string UserUpdatedMessage = "User Updated";
        internal const string UserNotUpdatedMessage = "User not Updated";
        internal const string UserExistsMessage = "User Exists";
        internal const string UserDeletedMessage = "User Deleted";
        internal const string UserNotDeletedMessage = "User not Deleted";
        internal const string UserActivatedMessage = "User Activated";
        internal const string UserNotActivatedMessage = "User not Activated";
        internal const string UserDeactivatedMessage = "User Deactivated";
        internal const string UserNotDeactivatedMessage = "User not Deactivated";
        internal const string UserDoesNotExistMessage = "User does not Exist";
        internal const string UserNameUniqueMessage = "User Name not Unique";
        internal const string UserNameRequiredMessage = "User Name Required";
        internal const string UserNameInvalidMessage = "User Name accepts Alphanumeric and Special Characters except Double and Single Quotes";
        internal const string UserNameConfirmedMessage = "User Name Confirmed";
        internal const string EmailUniqueMessage = "Email not Unique";
        internal const string EmailRequiredMessage = "Email Required";
        internal const string NoUserIsUsingThisEmailMessage = "No User is using this Email";
        internal const string RolesAddedMessage = "Roles Added";
        internal const string RolesNotAddedMessage = "Roles not Added";
        internal const string RolesRemovedMessage = "Roles Removed";
        internal const string RolesNotRemovedMessage = "Roles not Removed";
        internal const string RolesInvalidMessage = "Roles Invalid";
        internal const string PasswordResetMessage = "Password Reset";
        internal const string PasswordNotResetMessage = "Password not Reset";
        internal const string EmailConfirmedMessage = "Email Confirmed";
        internal const string EmailNotConfirmedMessage = "Email not Confirmed";
        internal const string OldEmailConfirmedMessage = "Old Email Confirmed";
        internal const string OldEmailNotConfirmedMessage = "Old Email not Confirmed";
        internal const string ProcessedPasswordResetRequestMessage = "Processed Password Reset Request";
        internal const string ResentPasswordResetRequestMessage = "Resent Password Reset Request";
        internal const string UnableToProcessPasswordResetRequesMessage = "Unable to Process Password Reset Request";
        internal const string UserEmailNotConfirmedMessage = "User Email not Confirmed";
        internal const string PasswordResetRequestNotFoundMessage = "Password Reset Request not Found";
        internal const string NoOutstandingRequestToResetPassworMessage = "No Outstanding Request to Reset Password";
        internal const string EmailConfirmationEmailResentMessage = "Email Confirmation Email Resent";
        internal const string EmailConfirmationEmailNotResentMessage = "Email Confirmation Email not Resent";
        internal const string EmailConfirmationRequestNotFoundMessage = "No Outstanding Email Confirmation Request Found";
        internal const string EmailConfirmationRequestCancelledMessage = "Email Confirmation Request Cancelled";
        internal const string EmailConfirmationRequestNotCancelledMessage = "Email Confirmation Request not Cancelled";
        internal const string PasswordResetEmailResentMessage = "Password Reset Email Resent";
        internal const string PasswordResetEmailNotResentMessage = "Password Reset Email not Resent";
        internal const string PasswordResetRequestCancelledMessage = "Password Reset Request Cancelled";
        internal const string PasswordResetRequestNotCancelledMessage = "Password Reset Request not Cancelled";
        internal const string EmailRequestsNotFoundMessage = "Email Requests not Found";
        internal const string UserIsAlreadyAnAdminMessage = "User is Already an Admin";
        internal const string UserHasBeenPromotedToAdminMessage = "User has been Promoted to Admin";
        internal const string UserHasNotBeenPromotedToAdminMessage = "User has not been Promoted to Admin";
        internal const string UserDoesNotHaveAdminPrivilegesMessage = "User does not have Admin Privileges";
        internal const string SuperUserCannotBePromotedMessage = "Super User cannot be Promoted";
        internal const string SuperUserCannotBeDeletedMessage = "Super User cannot be Deleted";
    }
}
