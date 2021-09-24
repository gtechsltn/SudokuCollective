namespace SudokuCollective.Core.Interfaces.Models.TokenModels
{
    public interface ITokenManagement
    {
        string Secret { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}
