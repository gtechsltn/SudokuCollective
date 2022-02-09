using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class EmailConfirmation : IEmailConfirmation
    {
        #region Fields
        private string _token = string.Empty;
        private string _oldEmailAddress = string.Empty;
        private string _newEmailAddress = string.Empty;
        private readonly EmailValidatedAttribute _emailValidatedAttribute = new();
        private readonly GuidValidatedAttribute _guidValidator = new();
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int AppId { get; set; }
        [Required, GuidValidated(ErrorMessage = AttributeMessages.InvalidToken)]
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _guidValidator.IsValid(value))
                {
                    _token = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidToken);
                }
            }
        }
        [EmailValidated(ErrorMessage = AttributeMessages.InvalidOldEmail)]
        public string OldEmailAddress
        {
            get
            {
                return _oldEmailAddress;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) && _emailValidatedAttribute.IsValid(value))
                {
                    _oldEmailAddress = value;
                    OldEmailAddressConfirmed = false;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidOldEmail);
                }
            }
        }
        [EmailValidated(ErrorMessage = AttributeMessages.InvalidNewEmail)]
        public string NewEmailAddress
        {
            get
            {
                return _newEmailAddress;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) && _emailValidatedAttribute.IsValid(value))
                {
                    _newEmailAddress = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidNewEmail);
                }
            }
        }
        public bool? OldEmailAddressConfirmed { get; set; }
        [Required]
        public bool IsUpdate
        {
            get
            {
                if (string.IsNullOrEmpty(OldEmailAddress) && string.IsNullOrEmpty(NewEmailAddress))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        [Required]
        public DateTime DateCreated { get; set; }
        #endregion

        #region Constructors
        public EmailConfirmation()
        {
            Id = 0;
            UserId = 0;
            AppId = 0;
            OldEmailAddressConfirmed = null;
            DateCreated = DateTime.MinValue;
        }

        public EmailConfirmation(int userId, int appId) : this()
        {
            UserId = userId;
            AppId = appId;
            Token = Guid.NewGuid().ToString();
            DateCreated = DateTime.UtcNow;
        }

        public EmailConfirmation(int userId, int appId, string oldEmailAddress, string newEmailAddress) : this()
        {
            UserId = userId;
            AppId = appId;
            Token = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(oldEmailAddress))
            {
                OldEmailAddress = oldEmailAddress;
            }
            if (!string.IsNullOrEmpty(newEmailAddress))
            {
                NewEmailAddress = newEmailAddress;
            }
            OldEmailAddressConfirmed = false;
            DateCreated = DateTime.UtcNow;
        }

        [JsonConstructor]
        public EmailConfirmation(
            int id,
            int userId,
            int appId,
            string token,
            string oldEmailAddress,
            string newEmailAddress,
            bool oldEmailAddressConfirmed,
            DateTime dateCreated)
        {
            Id = id;
            UserId = userId;
            AppId = appId;
            if (!string.IsNullOrEmpty(oldEmailAddress))
            {
                Token = token;
            }
            if (!string.IsNullOrEmpty(oldEmailAddress))
            {
                OldEmailAddress = oldEmailAddress;
            }
            if (!string.IsNullOrEmpty(newEmailAddress))
            {
                NewEmailAddress = newEmailAddress;
            }
            OldEmailAddressConfirmed = oldEmailAddressConfirmed;
            DateCreated = dateCreated;
        }
        #endregion

        #region Methods
        public override string ToString() => string.Format(base.ToString() + ".Id:{0}.AppId:{1}.UserId:{2}", Id, AppId, UserId);

        public string ToJson() => JsonSerializer.Serialize(
            this,
            new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        #endregion
    }
}
