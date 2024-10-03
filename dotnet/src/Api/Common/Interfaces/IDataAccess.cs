namespace CleanSlice.Api.Common.Interfaces
{
    public interface IDataAccess<TDomain, TAggregateRoot>
    {
        TDomain Retrieve();

        Task<TAggregateRoot> SaveAsync(TDomain domain, CancellationToken cancellationToken);
    }
}
