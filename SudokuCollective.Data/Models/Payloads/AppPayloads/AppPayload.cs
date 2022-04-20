using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class AppPayload : IAppPayload
    {
        [Required, JsonPropertyName("license")]
        public string Name { get; set; }
        [JsonPropertyName("localUrl")]
        public string LocalUrl { get; set; }
        [JsonPropertyName("stagingUrl")]
        public string StagingUrl { get; set; }
        [JsonPropertyName("qaUrl")]
        public string QaUrl { get; set; }
        [JsonPropertyName("prodUrl")]
        public string ProdUrl { get; set; }
        [Required, JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [Required, JsonPropertyName("environment")]
        public ReleaseEnvironment Environment { get; set; }
        [Required, JsonPropertyName("permitSuperUserAccess")]
        public bool PermitSuperUserAccess { get; set; }
        [Required, JsonPropertyName("permitCollectiveLogins")]
        public bool PermitCollectiveLogins { get; set; }
        [Required, JsonPropertyName("disableCustomUrls")]
        public bool DisableCustomUrls { get; set; }
        [Required, JsonPropertyName("customEmailConfirmationAction")]
        public string CustomEmailConfirmationAction { get; set; }
        [Required, JsonPropertyName("customPasswordResetAction")]
        public string CustomPasswordResetAction { get; set; }
        [Required, JsonPropertyName("timeFrame")]
        public TimeFrame TimeFrame { get; set; }
        [Required, JsonPropertyName("accessDuration")]
        public int AccessDuration { get; set; }

        public AppPayload()
        {
            Name = string.Empty;
            LocalUrl = string.Empty;
            StagingUrl = string.Empty;
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
            string stagingUrl, 
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
            StagingUrl = stagingUrl;
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
            string stagingUrl, 
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
            StagingUrl = stagingUrl;
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
