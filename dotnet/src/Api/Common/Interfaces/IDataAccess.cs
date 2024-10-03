namespace CleanSlice.Api.Common.Interfaces
{
    /// <summary>
    /// The repository that generates the aggregate root for a particular use case
    /// </summary>
    /// <typeparam name="TContext">The context of the command or request that can use to derive the aggregates</typeparam>
    /// <typeparam name="TDomain">The aggregate root for the use case</typeparam>
    /// <typeparam name="TSaveResult">The outcome of the saved domain state change</typeparam>
    public interface IDataAccess<TContext, TDomain, TSaveResult>
    {
        Task<TDomain> RetrieveAsync(TContext context, CancellationToken cancellationToken);

        Task<TSaveResult> SaveAsync(TDomain domain, CancellationToken cancellationToken);
    }
}
