namespace CleanSlice.Api.Common.Interfaces
{
    public interface IDbAccess<TDomain, TAggregateRoot>
    {
        TDomain Retrieve();

        Task<TAggregateRoot> AddAsync(TDomain domain, CancellationToken cancellationToken);
    }
}
