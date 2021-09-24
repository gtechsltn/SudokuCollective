namespace SudokuCollective.Core.Interfaces.Models.DomainEntities
{
    public interface IAvailableValue : IDomainObject
    {
        int Value { get; set; }
        bool Available { get; set; }
    }
}
