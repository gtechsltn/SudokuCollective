namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IEmailService : IService
    {
        bool Send(string to, string subject, string html);
    }
}
