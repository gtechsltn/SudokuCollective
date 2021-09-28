using System.Diagnostics.CodeAnalysis;

namespace SudokuCollective.Data.Messages
{
    public class UsersMessages
    {
        public const string UserFoundMessage = "User Found";
        public const string UsersFoundMessage = "Users Found";
        public const string UserNotFoundMessage = "User not Found";
        public const string UsersNotFoundMessage = "Users not Found";
        public const string UserCreatedMessage = "User Created";
        public const string UserNotCreatedMessage = "User not Created";
        public const string UserUpdatedMessage = "User Updated";
        public const string UserNotUpdatedMessage = "User not Updated";
        public const string UserExistsMessage = "User Exists";
        public const string UserDeletedMessage = "User Deleted";
        public const string UserNotDeletedMessage = "User not Deleted";
        public const string UserActivatedMessage = "User Activated";
        public const string UserNotActivatedMessage = "User not Activated";
        public const string UserDeactivatedMessage = "User Deactivated";
        public const string UserNotDeactivatedMessage = "User not Deactivated";
        public const string UserDoesNotExistMessage = "User does not Exist";
        public const string UserNameUniqueMessage = "User Name not Unique";
        public const string UserNameRequiredMessage = "User Name Required";
        public const string UserNameInvalidMessage = "User Name accepts Alphanumeric and Special Characters except Double and Single Quotes";
        public const string UserNameConfirmedMessage = "User Name Confirmed";
        public const string EmailUniqueMessage = "Email not Unique";
        public const string EmailRequiredMessage = "Email Required";
        public const string NoUserIsUsingThisEmailMessage = "No User is using this Email";
        public const string RolesAddedMessage = "Roles Added";
        public const string RolesNotAddedMessage = "Roles not Added";
        public const string RolesRemovedMessage = "Roles Removed";
        public const string RolesNotRemovedMessage = "Roles not Removed";
        public const string RolesInvalidMessage = "Roles Invalid";
        public const string PasswordResetMessage = "Password Reset";
        public const string PasswordNotResetMessage = "Password not Reset";
        public const string EmailConfirmedMessage = "Email Confirmed";
        public const string EmailNotConfirmedMessage = "Email not Confirmed";
        public const string OldEmailConfirmedMessage = "Old Email Confirmed";
        public const string OldEmailNotConfirmedMessage = "Old Email not Confirmed";
        public const string ProcessedPasswordResetRequestMessage = "Processed Password Reset Request";
        public const string ResentPasswordResetRequestMessage = "Resent Password Reset Request";
        public const string UnableToProcessPasswordResetRequesMessage = "Unable to Process Password Reset Request";
        public const string UserEmailNotConfirmedMessage = "User Email not Confirmed";
        public const string PasswordResetRequestNotFoundMessage = "Password Reset Request not Found";
        public const string NoOutstandingRequestToResetPassworMessage = "No Outstanding Request to Reset Password";
        public const string EmailConfirmationEmailResentMessage = "Email Confirmation Email Resent";
        public const string EmailConfirmationEmailNotResentMessage = "Email Confirmation Email not Resent";
        public const string EmailConfirmationRequestNotFoundMessage = "No Outstanding Email Confirmation Request Found";
        public const string EmailConfirmationRequestCancelledMessage = "Email Confirmation Request Cancelled";
        public const string EmailConfirmationRequestNotCancelledMessage = "Email Confirmation Request not Cancelled";
        public const string PasswordResetEmailResentMessage = "Password Reset Email Resent";
        public const string PasswordResetEmailNotResentMessage = "Password Reset Email not Resent";
        public const string PasswordResetRequestCancelledMessage = "Password Reset Request Cancelled";
        public const string PasswordResetRequestNotCancelledMessage = "Password Reset Request not Cancelled";
        public const string EmailRequestsNotFoundMessage = "Email Requests not Found";
        public const string UserIsAlreadyAnAdminMessage = "User is Already an Admin";
        public const string UserHasBeenPromotedToAdminMessage = "User has been Promoted to Admin";
        public const string UserHasNotBeenPromotedToAdminMessage = "User has not been Promoted to Admin";
        public const string UserDoesNotHaveAdminPrivilegesMessage = "User does not have Admin Privileges";
        public const string SuperUserCannotBePromotedMessage = "Super User cannot be Promoted";
        public const string SuperUserCannotBeDeletedMessage = "Super User cannot be Deleted";
    }
}
