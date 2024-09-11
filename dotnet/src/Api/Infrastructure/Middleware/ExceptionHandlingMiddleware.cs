using CleanSlice.Api.Common.DTOs;
using CleanSlice.Api.Common.Exceptions;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ILogger = Serilog.ILogger;

namespace CleanSlice.Api.Infrastructure.Middleware
{
    internal class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(ILogger Logger)
        {
            _logger = Logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                LogException(context, e);
                await HandleException(context, e);
            }
        }

        private void LogException(HttpContext context, Exception exception)
        {
            switch (exception)
            {
                case ApiException:
                case ValidationException:
                    _logger.Warning(exception, "Error handling {RequestMethod} {RequestUrl}", context.Request.Method, context.Request.Path);
                    break;
                default:
                    _logger.Error(exception, "Error handling  {RequestMethod} {RequestUrl}", context.Request.Method, context.Request.Path);
                    break;
            };
        }

        private static async Task HandleException(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);

            ErrorResponse response = new()
            {
                Title = GetTitle(exception),
                Detail = statusCode == StatusCodes.Status500InternalServerError ? "An error occurred and we're working hard to get this working for you again" : exception.Message,
                Errors = statusCode == StatusCodes.Status500InternalServerError ? new Dictionary<string, string[]>() : GetErrors(exception)
            };

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }));
        }

        private static string GetTitle(Exception exception)
        {
            return exception switch
            {
                ApiException apie => apie.Message,
                ValidationException ve => ve.Message,
                BadHttpRequestException ve => "Bad request made. Please check your request again.",
                _ => "Server Error"
            };
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {

                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                BadHttpRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
        {
            Dictionary<string, string[]> dict = new();
            if (exception == null)
            {
                return dict;
            }

            if (exception is ValidationException validationException)
            {
                Dictionary<string, string[]> errors = validationException.Errors
                .GroupBy(err => err.PropertyName, err => err.ErrorMessage)
                .ToDictionary(grp => grp.Key, grp => grp.ToArray());

                dict = dict.Union(errors).GroupBy(d => d.Key)
                        .ToDictionary(d => d.Key, d => d.SelectMany(x => x.Value).ToArray());
            }
            else
            {
                dict.Add(exception.Message, Array.Empty<string>());

                if (exception.InnerException != null)
                {
                    foreach (KeyValuePair<string, string[]> innerItem in GetErrors(exception.InnerException))
                    {
                        dict.Add(innerItem.Key, innerItem.Value);
                    }
                }
            }

            return dict;
        }
    }
}
