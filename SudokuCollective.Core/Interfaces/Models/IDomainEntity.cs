namespace SudokuCollective.Core.Interfaces.Models
{
    public interface IDomainEntity : IDomainObject
    {
        int Id { get; set; }
    }
}
