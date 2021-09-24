namespace SudokuCollective.Core.Interfaces.DataModels
{
    public interface ITokenManagement
    {
        string Secret { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}
