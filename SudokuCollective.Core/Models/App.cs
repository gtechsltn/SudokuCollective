using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class App : IApp
    {
        #region Fields
        private string _license = string.Empty;
        private TimeFrame _timeFrame = TimeFrame.NULL;
        private int _accessDuration = 0;
        private readonly GuidValidatedAttribute _guidValidator = new();
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [IgnoreDataMember, GuidValidated(ErrorMessage = "Guid must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters")]
        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _guidValidator.IsValid(value))
                {
                    _license = value;
                }
                else
                {
                    throw new ArgumentException("Guid must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters");
                }
            }
        }
        [Required]
        public int OwnerId { get; set; }
        public string LocalUrl { get; set; }
        public string DevUrl { get; set; }
        public string QaUrl { get; set; }
        public string ProdUrl { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public ReleaseEnvironment Environment { get; set; }
        [Required]
        public bool PermitSuperUserAccess { get; set; }
        [Required]
        public bool PermitCollectiveLogins { get; set; }

        [IgnoreDataMember]
        public bool UseCustomEmailConfirmationAction
        {
            get
            {
                if (
                    Environment == ReleaseEnvironment.LOCAL
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LocalUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.DEV
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(DevUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.QA
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(QaUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.PROD
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(ProdUrl)
                    && !string.IsNullOrEmpty(CustomEmailConfirmationAction)
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [IgnoreDataMember]
        public bool UseCustomPasswordResetAction
        {
            get
            {
                if (
                    Environment == ReleaseEnvironment.LOCAL
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(LocalUrl)
                    && !string.IsNullOrEmpty(CustomPasswordResetAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.DEV
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(DevUrl)
                    && !string.IsNullOrEmpty(CustomPasswordResetAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.QA
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(QaUrl)
                    && !string.IsNullOrEmpty(CustomPasswordResetAction)
                )
                {
                    return true;
                }
                else if (
                    Environment == ReleaseEnvironment.PROD
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(ProdUrl)
                    && !string.IsNullOrEmpty(CustomPasswordResetAction)
                )
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
        [Required]
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
            get { return _timeFrame; }
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
                else { }
            }
        }
        public int AccessDuration
        {
            get { return _accessDuration; }
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
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }

        [JsonIgnore]
        ICollection<IUserApp> IApp.Users
        {
            get { return Users.ConvertAll(u => (IUserApp)u); }
            set { Users = value.ToList().ConvertAll(u => (UserApp)u); }
        }
        [Required]
        public virtual List<UserApp> Users { get; set; }
        #endregion

        #region Constructors
        public App()
        {
            Id = 0;
            Name = string.Empty;
            OwnerId = 0;
            DateCreated = DateTime.UtcNow;
            LocalUrl = string.Empty;
            DevUrl = string.Empty;
            QaUrl = string.Empty;
            ProdUrl = string.Empty;
            IsActive = false;
            PermitSuperUserAccess = false;
            PermitCollectiveLogins = false;
            Environment = ReleaseEnvironment.LOCAL;
            DisableCustomUrls = true;
            CustomEmailConfirmationAction = string.Empty;
            CustomPasswordResetAction = string.Empty;
            Users = new List<UserApp>();
            TimeFrame = TimeFrame.DAYS;
            AccessDuration = 1;
        }

        public App(string name, string license, int ownerId, string devUrl, string prodUrl) : this()
        {
            Name = name;
            License = license;
            OwnerId = ownerId;
            DateCreated = DateTime.UtcNow;
            DevUrl = devUrl;
            ProdUrl = prodUrl;
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
            string prodUrl,
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
            DateTime dateUpdated
        )
        {
            Id = id;
            Name = name;
            License = license;
            OwnerId = ownerId;
            LocalUrl = localUrl;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
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
            Users = new List<UserApp>();
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
