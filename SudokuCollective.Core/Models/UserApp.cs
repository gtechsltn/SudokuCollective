using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class UserApp : IUserApp
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [IgnoreDataMember]
        IUser IUserApp.User
        {
            get
            {
                return User;
            }
            set
            {
                User = (User)value;
            }
        }
        [IgnoreDataMember]
        public virtual User User { get; set; }
        [Required]
        public int AppId { get; set; }
        [IgnoreDataMember]
        IApp IUserApp.App
        {
            get
            {
                return App;
            }
            set
            {
                App = (App)value;
            }
        }
        [IgnoreDataMember]
        public virtual App App { get; set; }
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
