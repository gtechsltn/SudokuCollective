using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Utilities;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class App : IApp
    {
        #region Fields
        private string _license = string.Empty;
        private string _localUrl = string.Empty;
        private string _stagingUrl = string.Empty;
        private string _qaUrl = string.Empty;
        private string _prodUrl = string.Empty;
        private TimeFrame _timeFrame = TimeFrame.NULL;
        private int _accessDuration = 0;
        private readonly GuidValidatedAttribute _guidValidator = new();
        private readonly UrlValidatedAttribute _urlValidator = new();
        #endregion

        #region Properties
        [Required, JsonPropertyName("id")]
        public int Id { get; set; }
        [Required, JsonPropertyName("name")]
        public string Name { get; set; }
        [IgnoreDataMember, GuidValidated(ErrorMessage = AttributeMessages.InvalidLicense)]
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
                    throw new ArgumentException(AttributeMessages.InvalidLicense);
                }
            }
        }
        [Required, JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }
        [JsonPropertyName("localUrl"), UrlValidated(ErrorMessage = AttributeMessages.InvalidUrl)]
        public string LocalUrl
        {
            get
            {
                return _localUrl;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _urlValidator.IsValid(value))
                {
                    _localUrl = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidUrl);
                }
            }
        }
        [JsonPropertyName("stagingUrl"), UrlValidated(ErrorMessage = AttributeMessages.InvalidUrl)]
        public string StagingUrl
        {
            get
            {
                return _stagingUrl;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _urlValidator.IsValid(value))
                {
                    _stagingUrl = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidUrl);
                }
            }
        }
        [JsonPropertyName("qaUrl"), UrlValidated(ErrorMessage = AttributeMessages.InvalidUrl)]
        public string QaUrl
        {
            get
            {
                return _qaUrl;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _urlValidator.IsValid(value))
                {
                    _qaUrl = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidUrl);
                }
            }
        }
        [JsonPropertyName("prodUrl"), UrlValidated(ErrorMessage = AttributeMessages.InvalidUrl)]
        public string ProdUrl
        {
            get
            {
                return _prodUrl;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _urlValidator.IsValid(value))
                {
                    _prodUrl = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidUrl);
                }
            }
        }
        [Required, JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }
        [Required, JsonPropertyName("environment")]
        public ReleaseEnvironment Environment { get; set; }
        [Required, JsonPropertyName("permitSuperUserAccess")]
        public bool PermitSuperUserAccess { get; set; }
        [Required, JsonPropertyName("permitCollectiveLogins")]
        public bool PermitCollectiveLogins { get; set; }
        [JsonIgnore]
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
                    Environment == ReleaseEnvironment.STAGING
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(StagingUrl)
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
        [JsonIgnore]
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
                    Environment == ReleaseEnvironment.STAGING
                    && !DisableCustomUrls
                    && !string.IsNullOrEmpty(StagingUrl)
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
        [JsonPropertyName("disableCustomUrls")]
        public bool DisableCustomUrls { get; set; }
        [JsonPropertyName("customEmailConfirmationAction")]
        public string CustomEmailConfirmationAction { get; set; }
        [JsonPropertyName("customPasswordResetAction")]
        public string CustomPasswordResetAction { get; set; }
        [Required, JsonPropertyName("userCount")]
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
        [JsonPropertyName("timeFrame")]
        public TimeFrame TimeFrame
        {
            get { return _timeFrame; }
            set
            {
                _timeFrame = value;


                if (value == TimeFrame.SECONDS && AccessDuration > 60)
                {
                    AccessDuration = 60;
                }
                else if (value == TimeFrame.MINUTES && AccessDuration > 60)
                {
                    AccessDuration = 60;
                }
                else if (value == TimeFrame.HOURS && AccessDuration > 23)
                {
                    AccessDuration = 23;
                }
                else if (value == TimeFrame.DAYS && AccessDuration > 31)
                {
                    AccessDuration = 31;
                }
                else if (value == TimeFrame.MONTHS && AccessDuration > 12)
                {
                    AccessDuration = 12;
                }
                else if (value == TimeFrame.YEARS && AccessDuration > 5)
                {
                    AccessDuration = 5;
                }
            }
        }
        [JsonPropertyName("accessDuration")]
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
                else if (TimeFrame == TimeFrame.MONTHS)
                {
                    if (0 < value || value <= 12)
                    {
                        _accessDuration = value;
                    }
                }
                else if (TimeFrame == TimeFrame.YEARS)
                {
                    if (0 < value || value <= 5)
                    {
                        _accessDuration = value;
                    }
                }
            }
        }
        [Required, JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }
        [Required, JsonPropertyName("dateUpdated")]
        public DateTime DateUpdated { get; set; }
        [JsonIgnore]
        ICollection<IUserApp> IApp.Users
        {
            get { return Users.ConvertAll(u => (IUserApp)u); }
            set { Users = value.ToList().ConvertAll(u => (UserApp)u); }
        }
        [Required, JsonPropertyName("users"), JsonConverter(typeof(IDomainEntityListConverter<List<UserApp>>))]
        public virtual List<UserApp> Users { get; set; }
        #endregion

        #region Constructors
        public App()
        {
            Id = 0;
            Name = string.Empty;
            OwnerId = 0;
            DateCreated = DateTime.UtcNow;
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

        public App(string name, string license, int ownerId, string stagingUrl, string prodUrl) : this()
        {
            Name = name;
            License = license;
            OwnerId = ownerId;
            DateCreated = DateTime.UtcNow;
            StagingUrl = stagingUrl;
            ProdUrl = prodUrl;
            IsActive = true;
        }

        [JsonConstructor]
        public App(
            int id,
            string name,
            string license,
            int ownerId,
            string localUrl,
            string stagingUrl,
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
            StagingUrl = stagingUrl;
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

        public override string ToString() => string.Format(base.ToString() + ".Id:{0}.Name:{1}", Id, Name);

        public string ToJson() => JsonSerializer.Serialize(
            this,
            new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        #endregion
    }
}
