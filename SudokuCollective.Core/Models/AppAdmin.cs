using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class AppAdmin : IAppAdmin
    {
        #region Properties
        public int Id { get; set; }
        public int AppId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        #endregion

        #region Constructors
        public AppAdmin()
        {
            Id = 0;
            AppId = 0;
            UserId = 0;
            IsActive = true;
        }

        public AppAdmin(int appId, int userId) : this()
        {
            Id = 0;
            AppId = appId;
            UserId = userId;
        }

        [JsonConstructor]
        public AppAdmin(
            int id,
            int appId,
            int userId,
            bool isActive)
        {
            Id = id;
            AppId = appId;
            UserId = userId;
            IsActive = isActive;
        }
        #endregion
    }
}
