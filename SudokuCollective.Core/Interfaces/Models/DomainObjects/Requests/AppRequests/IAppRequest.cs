using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IAppRequest
    {
        string Name { get; set; }
        string LocalUrl { get; set; }
        string DevUrl { get; set; }
        string QaUrl { get; set; }
        string LiveUrl { get; set; }
        bool IsActive { get; set; }
        ReleaseEnvironment Environment { get; set; }
        bool PermitSuperUserAccess { get; set; }
        bool PermitCollectiveLogins { get; set; }
        bool DisableCustomUrls { get; set; }
        string CustomEmailConfirmationAction { get; set; }
        string CustomPasswordResetAction { get; set; }
        TimeFrame TimeFrame { get; set; }
        int AccessDuration { get; set; }
    }
}
