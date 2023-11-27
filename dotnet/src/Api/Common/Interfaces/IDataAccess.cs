namespace CleanSlice.Api.Common.Interfaces
{
    public interface IDataAccess<TDomain, TAggregateRoot>
    {
        TDomain Retrieve();

        Task<TAggregateRoot> AddAsync(TDomain domain, CancellationToken cancellationToken);
    }
}
