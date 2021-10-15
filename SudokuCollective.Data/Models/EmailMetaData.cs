using SudokuCollective.Core.Interfaces.DataModels;

namespace SudokuCollective.Data.Models
{
    public class EmailMetaData : IEmailMetaData
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
    }
}
