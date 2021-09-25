using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class App : IApp
    {
        #region Fields
        private List<IUserApp> _users;
        private TimeFrame _timeFrame;
        private int _accessDuration;
        #endregion

        #region Properties
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        [Required]
        public string License { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public string LocalUrl { get; set; }
        public string DevUrl { get; set; }
        public string QaUrl { get; set; }
        public string LiveUrl { get; set; }
        public bool IsActive { get; set; }
        public ReleaseEnvironment Environment { get; set; }
        public bool PermitSuperUserAccess { get; set; }
        public bool PermitCollectiveLogins { get; set; }

        [JsonIgnore]
        public bool UseCustomEmailConfirmationAction
        {
            get
            {
                if (Environment == ReleaseEnvironment.LOCAL
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LocalUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.DEV
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(DevUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.QA
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(QaUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.PROD
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LiveUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [JsonIgnore]
        public bool UseCustomPasswordResetAction
        {
            get
            {
                if (Environment == ReleaseEnvironment.LOCAL
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LocalUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.DEV
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(DevUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.QA
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(QaUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                if (Environment == ReleaseEnvironment.PROD
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LiveUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DisableCustomUrls { get; set; }
        public string CustomEmailConfirmationAction { get; set; }
        public string CustomPasswordResetAction { get; set; }

        public int UserCount
        {
            get
            {
                if (Users != null)
                {
                    return Users.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public TimeFrame TimeFrame
        {
            get
            {
                return _timeFrame;
            }

            set
            {
                _timeFrame = value;

                if (value == TimeFrame.HOURS && AccessDuration < 23)
                {
                    AccessDuration = 23;
                }
                else if (value == TimeFrame.DAYS && AccessDuration < 31)
                {
                    AccessDuration = 31;
                }
                else if (value == TimeFrame.MONTHS && AccessDuration < 12)
                {
                    AccessDuration = 12;
                }
                else
                {

                }
            }
        }
        public int AccessDuration
        {
            get
            {
                return _accessDuration;
            }

            set
            {
                if (TimeFrame == TimeFrame.SECONDS)
                {
                    if (0 < value || value <= 59)
                    {
                        _accessDuration = value;
                    }
                }
                else if (TimeFrame == TimeFrame.MINUTES)
                {
                    if (0 < value || value <= 59)
                    {
                        _accessDuration = value;
                    }
                }
                else if (TimeFrame == TimeFrame.HOURS)
                {
                    if (0 < value || value <= 23)
                    {
                        _accessDuration = value;
                    }
                }
                else if (TimeFrame == TimeFrame.DAYS)
                {
                    if (0 < value || value <= 31)
                    {
                        _accessDuration = value;
                    }
                }
                else
                {
                    if (0 < value || value <= 12)
                    {
                        _accessDuration = value;
                    }
                }
            }
        }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public List<IUserApp> Users
        {

            get
            {
                return _users;
            }

            set
            {
                _users = value;
            }
        }
        #endregion

        #region Constructors
        public App()
        {
            _users = new List<IUserApp>();

            Id = 0;
            Name = string.Empty;
            License = string.Empty;
            OwnerId = 0;
            DateCreated = DateTime.UtcNow;
            LocalUrl = string.Empty;
            DevUrl = string.Empty;
            QaUrl = string.Empty;
            LiveUrl = string.Empty;
            IsActive = false;
            PermitSuperUserAccess = false;
            PermitCollectiveLogins = false;
            Environment = ReleaseEnvironment.LOCAL;
            DisableCustomUrls = true;
            CustomEmailConfirmationAction = string.Empty;
            CustomPasswordResetAction = string.Empty;
            Users = new List<IUserApp>();
            TimeFrame = TimeFrame.DAYS;
            AccessDuration = 1;
        }

        public App(
            string name,
            string license,
            int ownerId,
            string devUrl,
            string liveUrl) : this()
        {
            _users = new List<IUserApp>();

            Name = name;
            License = license;
            OwnerId = ownerId;
            DateCreated = DateTime.UtcNow;
            DevUrl = devUrl;
            LiveUrl = liveUrl;
        }

        [JsonConstructor]
        public App(
            int id,
            string name,
            string license,
            int ownerId,
            string localUrl,
            string devUrl,
            string qaUrl,
            string liveUrl,
            bool isActive,
            bool permitSuperUserAccess,
            bool permitCollectiveLogins,
            ReleaseEnvironment environment,
            bool disableCustomUrls,
            string customEmailConfirmationAction,
            string customPasswordResetAction,
            TimeFrame timeFrame,
            int accessDuration,
            DateTime dateCreated,
            DateTime dateUpdated)
        {
            _users = new List<IUserApp>();

            Id = id;
            Name = name;
            License = license;
            OwnerId = ownerId;
            LocalUrl = localUrl;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            LiveUrl = liveUrl;
            IsActive = isActive;
            PermitSuperUserAccess = permitSuperUserAccess;
            PermitCollectiveLogins = permitCollectiveLogins;
            Environment = environment;
            DisableCustomUrls = disableCustomUrls;
            CustomEmailConfirmationAction = customEmailConfirmationAction;
            CustomPasswordResetAction = customPasswordResetAction;
            TimeFrame = timeFrame;
            AccessDuration = accessDuration;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
        }
        #endregion

        #region Methods
        public void ActivateApp()
        {
            IsActive = true;
        }

        public void DeactivateApp()
        {
            IsActive = false;
        }

        public string GetLicense(int id, int ownerId)
        {
            var result = string.Empty;

            if (Id == id && OwnerId == id)
            {
                result = License;
            }

            return result;
        }
        #endregion
    }
}
