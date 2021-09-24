using System;
using System.Collections.Generic;
using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainEntities
{
    public interface IApp : IDomainEntity
    {
        string Name { get; set; }
        string License { get; set; }
        int OwnerId { get; set; }
        string DevUrl { get; set; }
        string LiveUrl { get; set; }
        bool IsActive { get; set; }
        bool InDevelopment { get; set; }
        bool PermitSuperUserAccess { get; set; }
        bool PermitCollectiveLogins { get; set; }
        bool UseCustomEmailConfirmationAction { get; }
        bool UseCustomPasswordResetAction { get; }
        bool DisableCustomUrls { get; set; }
        string CustomEmailConfirmationAction { get; set; }
        string CustomPasswordResetAction { get; set; }
        int UserCount { get; }
        TimeFrame TimeFrame { get; set; }
        int AccessDuration { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        List<IUserApp> Users { get; set; }
        public string GetLicense(int id, int ownerId);
        public void ActivateApp();
        public void DeactivateApp();
    }
}
