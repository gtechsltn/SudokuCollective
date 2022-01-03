using System;
using System.Collections.Generic;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Core.Interfaces.Models.DomainEntities
{
    public interface IUser : IDomainEntity
    {
        string UserName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string NickName { get; set; }
        string FullName { get; }
        string Email { get; set; }
        bool IsEmailConfirmed { get; set; }
        bool ReceivedRequestToUpdateEmail { get; set; }
        string Password { get; set; }
        bool ReceivedRequestToUpdatePassword { get; set; }
        bool IsActive { get; set; }
        bool IsSuperUser { get; set; }
        bool IsAdmin { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        List<Game> Games { get; set; }
        List<UserRole> Roles { get; set; }
        List<UserApp> Apps { get; set; }
        void ActivateUser();
        void DeactiveUser();
        void UpdateRoles();
    }
}
