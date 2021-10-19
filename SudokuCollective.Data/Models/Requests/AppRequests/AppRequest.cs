using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class AppRequest : IAppRequest
    {
        public string Name { get; set; }
        public string LocalUrl { get; set; }
        public string DevUrl { get; set; }
        public string QaUrl { get; set; }
        public string LiveUrl { get; set; }
        public bool IsActive { get; set; }
        public ReleaseEnvironment Environment { get; set; }
        public bool PermitSuperUserAccess { get; set; }
        public bool PermitCollectiveLogins { get; set; }
        public bool DisableCustomUrls { get; set; }
        public string CustomEmailConfirmationAction { get; set; }
        public string CustomPasswordResetAction { get; set; }
        public TimeFrame TimeFrame { get; set; }
        public int AccessDuration { get; set; }

        public AppRequest()
        {
            Name = string.Empty;
            LocalUrl = string.Empty;
            DevUrl = string.Empty;
            QaUrl = string.Empty;
            LiveUrl = string.Empty;
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

        public AppRequest(
            string name, 
            string localUrl, 
            string devUrl, 
            string qaUrl, 
            string liveUrl, 
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
            LiveUrl = liveUrl;
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

        public AppRequest(
            string name, 
            string localUrl, 
            string devUrl, 
            string qaUrl, 
            string liveUrl, 
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
            LiveUrl = liveUrl;
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
    }
}
