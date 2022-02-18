using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class AppPayload : IAppPayload
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LocalUrl { get; set; }
        [Required]
        public string DevUrl { get; set; }
        [Required]
        public string QaUrl { get; set; }
        [Required]
        public string ProdUrl { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public ReleaseEnvironment Environment { get; set; }
        [Required]
        public bool PermitSuperUserAccess { get; set; }
        [Required]
        public bool PermitCollectiveLogins { get; set; }
        [Required]
        public bool DisableCustomUrls { get; set; }
        [Required]
        public string CustomEmailConfirmationAction { get; set; }
        [Required]
        public string CustomPasswordResetAction { get; set; }
        [Required]
        public TimeFrame TimeFrame { get; set; }
        [Required]
        public int AccessDuration { get; set; }

        public AppPayload()
        {
            Name = string.Empty;
            LocalUrl = string.Empty;
            DevUrl = string.Empty;
            QaUrl = string.Empty;
            ProdUrl = string.Empty;
            IsActive = false;
            Environment = ReleaseEnvironment.NULL;
            PermitSuperUserAccess = false;
            PermitCollectiveLogins = false;
            DisableCustomUrls = false;
            CustomEmailConfirmationAction = string.Empty;
            CustomPasswordResetAction = string.Empty;
            TimeFrame = TimeFrame.NULL;
            AccessDuration = 0;
        }

        public AppPayload(
            string name, 
            string localUrl, 
            string devUrl, 
            string qaUrl, 
            string prodUrl, 
            bool isActive, 
            int environment, 
            bool permitSuperUserAccess, 
            bool permitCollectiveLogins, 
            bool disableCustomUrls, 
            string customEmailConfirmationAction, 
            string customPasswordResetAction, 
            int timeFrame, 
            int accessDuration)
        {
            Name = name;
            LocalUrl = localUrl;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
            IsActive = isActive;
            Environment = (ReleaseEnvironment)environment;
            PermitSuperUserAccess = permitSuperUserAccess;
            PermitCollectiveLogins = permitCollectiveLogins;
            DisableCustomUrls = disableCustomUrls;
            CustomEmailConfirmationAction = customEmailConfirmationAction;
            CustomPasswordResetAction = customPasswordResetAction;
            TimeFrame = (TimeFrame)timeFrame;
            AccessDuration = accessDuration;
        }

        public AppPayload(
            string name, 
            string localUrl, 
            string devUrl, 
            string qaUrl, 
            string prodUrl, 
            bool isActive, 
            ReleaseEnvironment environment, 
            bool permitSuperUserAccess, 
            bool permitCollectiveLogins, 
            bool disableCustomUrls, 
            string customEmailConfirmationAction, 
            string customPasswordResetAction, 
            TimeFrame timeFrame, 
            int accessDuration)
        {
            Name = name;
            LocalUrl = localUrl;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
            IsActive = isActive;
            Environment = environment;
            PermitSuperUserAccess = permitSuperUserAccess;
            PermitCollectiveLogins = permitCollectiveLogins;
            DisableCustomUrls = disableCustomUrls;
            CustomEmailConfirmationAction = customEmailConfirmationAction;
            CustomPasswordResetAction = customPasswordResetAction;
            TimeFrame = timeFrame;
            AccessDuration = accessDuration;
        }

        public static implicit operator JsonElement(AppPayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
