using FluentValidation;
using MediatR;
using ILogger = Serilog.ILogger;

namespace CleanSlice.Api.Infrastructure.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger _logger;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger Logger)
        {
            _validators = validators;
            _logger = Logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                ValidationContext<TRequest> context = new(request);

                FluentValidation.Results.ValidationResult[] validationResults = await Task.WhenAll(_validators
                    .Select(v => v.ValidateAsync(context, cancellationToken)));

                List<FluentValidation.Results.ValidationFailure> failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                {
                    _logger.Information("Invalid request {Request} with {Failures} ", typeof(TRequest).Name, failures);
                    throw new ValidationException(failures);
                }
            }

            _logger.Information("Successful validated request {Request}", typeof(TResponse).Name);

            return await next();
        }
    }
}
