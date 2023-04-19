using MediatR;
using ILogger = Serilog.ILogger;

namespace CleanSlice.Api.Infrastructure.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _logger;
        public LoggingBehaviour(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.Information("Handling {Request} {@RequestBody}", typeof(TRequest).Name, request);

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            TResponse response = await next();
            watch.Stop();

            _logger.Information("Handled {Request}. {ElapseMs}", typeof(TResponse).Name, watch.ElapsedMilliseconds);

            return response;
        }
    }
}
