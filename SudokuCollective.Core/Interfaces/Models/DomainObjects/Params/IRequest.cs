namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Params
{
    public interface IRequest : IDomainObject
    {
        string License { get; set; }
        int RequestorId { get; set; }
        int AppId { get; set; }
        IPaginator Paginator { get; set; }
        IDomainObject DataPacket { get; set; }
    }
}
