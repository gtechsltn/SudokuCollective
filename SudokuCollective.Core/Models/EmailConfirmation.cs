using System;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class EmailConfirmation : IEmailConfirmation
    {
        #region Fields
        private string _oldEmailAddress;
        #endregion

        #region Properties
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AppId { get; set; }
        public string Token { get; set; }
        public string OldEmailAddress
        {
            get
            {
                return _oldEmailAddress;
            }

            set
            {
                _oldEmailAddress = value;
                OldEmailAddressConfirmed = false;
            }
        }
        public string NewEmailAddress { get; set; }
        public bool? OldEmailAddressConfirmed { get; set; }
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
        public DateTime DateCreated { get; set; }
        #endregion

        #region Constructors
        public EmailConfirmation()
        {
            Id = 0;
            UserId = 0;
            AppId = 0;
            Token = string.Empty;
            OldEmailAddress = string.Empty;
            NewEmailAddress = string.Empty;
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
            OldEmailAddress = oldEmailAddress;
            NewEmailAddress = newEmailAddress;
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
            Token = token;
            OldEmailAddress = oldEmailAddress;
            NewEmailAddress = newEmailAddress;
            OldEmailAddressConfirmed = oldEmailAddressConfirmed;
            DateCreated = dateCreated;
        }
        #endregion
    }
}
