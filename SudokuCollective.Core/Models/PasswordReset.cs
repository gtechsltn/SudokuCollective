using System;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class PasswordReset : IPasswordReset
    {
        #region Properties
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AppId { get; set; }
        public string Token { get; set; }
        public DateTime DateCreated { get; set; }
        #endregion

        #region Constructors
        public PasswordReset()
        {
            Id = 0;
            UserId = 0;
            AppId = 0;
            Token = string.Empty;
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
    }
}
