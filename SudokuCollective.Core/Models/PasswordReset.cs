using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class PasswordReset : IPasswordReset
    {
        #region Fields
        private string _token = string.Empty;
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
                var validator = new GuidValidatedAttribute();

                if (!string.IsNullOrEmpty(value) && validator.IsValid(value))
                {
                    _token = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidToken);
                }
            }
        }
        [Required]
        public DateTime DateCreated { get; set; }
        #endregion

        #region Constructors
        public PasswordReset()
        {
            Id = 0;
            UserId = 0;
            AppId = 0;
            DateCreated = DateTime.MinValue;
        }

        public PasswordReset(int userId, int appId) : this()
        {
            UserId = userId;
            AppId = appId;
            Token = Guid.NewGuid().ToString();
            DateCreated = DateTime.UtcNow;
        }

        [JsonConstructor]
        public PasswordReset(int id, int userId, int appId, string token, DateTime dateCreated)
        {
            Id = id;
            UserId = userId;
            AppId = appId;
            Token = token;
            DateCreated = dateCreated;
        }
        #endregion

        #region Methods
        public override string ToString() => string.Format(base.ToString() + ".Id:{0}.AppId:{1}.UserId:{2}", Id, AppId, UserId);

        public string ToJson() => JsonSerializer.Serialize(this);
        #endregion
    }
}
