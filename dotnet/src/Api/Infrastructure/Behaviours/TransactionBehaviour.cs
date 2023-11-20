using MediatR;

namespace CleanSlice.Api.Infrastructure.Behaviours
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        // private readonly ILogger _logger;
        // private readonly CleanSliceContext _dbContext;

        // public TransactionBehaviour(CleanSliceContext dbContext,
        //     ILogger logger)
        // {
        //     _dbContext = dbContext;
        //     _logger = logger;
        // }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            return await next();
            //     try
            //     {
            //         if (_dbContext.HasActiveTransaction)
            //         {
            //             return await next();
            //         }

            //         Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

            //         TResponse? response = default;
            //         await strategy.ExecuteAsync(async () =>
            //         {
            //             await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
            //             using (LogContext.PushProperty("TransactionId", transaction.TransactionId))
            //             {
            //                 _logger.Information("Begin transaction for {Request} ({@RequestBody})", typeof(TRequest).Name, request);

            //                 TResponse response = await next();

            //                 await _dbContext.CommitTransactionAsync(transaction);

            //                 _logger.Information("Commit transaction for {Request}", typeof(TRequest).Name);
            //             }
            //         });

            //         // If it is null here, most probably the response was not done in the above execution
            //         return response ?? await next();
            //     }
            //     catch (Exception ex)
            //     {
            //         _logger.Error(ex, "ERROR Handling transaction for {Request} ({@RequestBody})", typeof(TRequest).Name, request);

            //         throw;
            //     }
        }
    }
}
