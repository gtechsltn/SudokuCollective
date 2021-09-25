namespace SudokuCollective.Core.Interfaces.Models.TokenModels
{
    public interface ITokenRequest : IDomainObject
    {
        string UserName { get; set; }
        string Password { get; set; }
        string License { get; set; }
    }
}
