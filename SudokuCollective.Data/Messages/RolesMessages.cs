using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SudokuCollective.Repos")]
namespace SudokuCollective.Data.Messages
{
    internal static class RolesMessages
    {
        internal const string RoleFoundMessage = "Role Found";
        internal const string RoleNotFoundMessage = "Role not Found";
        internal const string RolesFoundMessage = "Roles Found";
        internal const string RolesNotFoundMessage = "Roles not Found";
        internal const string RoleCreatedMessage = "Role Created";
        internal const string RoleNotCreatedMessage = "Role not Created";
        internal const string RoleUpdatedMessage = "Role Updated";
        internal const string RoleNotUpdatedMessage = "Role not Updated";
        internal const string RoleDeletedMessage = "Role Deleted";
        internal const string RoleNotDeletedMessage = "Role not Deleted";
        internal const string RoleAlreadyExistsMessage = "Role Already Exists";
        internal const string RoleDoesNotExistMessage = "Role does not Exist";
    }
}
