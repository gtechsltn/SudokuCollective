using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class UserApp : IUserApp
    {
        #region Properties
        public int Id { get; set; }
        public int UserId { get; set; }
        [IgnoreDataMember]
        public IUser User { get; set; }
        public int AppId { get; set; }
        [IgnoreDataMember]
        public IApp App { get; set; }
        #endregion

        #region Constructors
        public UserApp()
        {
            Id = 0;
            UserId = 0;
            User = null;
            AppId = 0;
            App = null;
        }

        public UserApp(int userId, int appId) : this()
        {
            UserId = userId;
            AppId = appId;
        }

        [JsonConstructor]
        public UserApp(
            int id,
            int userId,
            int appId)
        {
            Id = id;
            UserId = userId;
            AppId = appId;
        }
        #endregion
    }
}
