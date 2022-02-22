namespace SudokuCollective.Core.Interfaces.ServiceModels
{
    public interface ITokenManagement
    {
        string Secret { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}
