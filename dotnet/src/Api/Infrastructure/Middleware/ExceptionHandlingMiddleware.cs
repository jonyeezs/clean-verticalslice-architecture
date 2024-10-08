using CleanSlice.Api.Common.DTOs;
using CleanSlice.Api.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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
                    _logger.Error(exception, "Error handling {RequestMethod} {RequestUrl}", context.Request.Method, context.Request.Path);
                    break;
            };
        }

        private async Task HandleException(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);

            ErrorResponse response = new()
            {
                Title = GetTitle(exception),
                Details = statusCode == StatusCodes.Status500InternalServerError ? new Dictionary<string, string[]>() : GetErrors(exception)
            };
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = statusCode;

            await WrapContextResponseForModifying(httpContext, async (resp) =>
            {
                var stream = resp.Body;
                stream.SetLength(0);
                using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(jsonResponse);
                await writer.FlushAsync();
                resp.ContentLength = stream.Length;
            });
        }

        /// <summary>
        /// Creates a temporary memory stream so we can write into the response otherwise the context stream does not support it.
        /// </summary>
        /// <param name="context">the HttpContext containing the response</param>
        /// <param name="modifyingInstructions"></param>
        /// <returns></returns>
        private async Task WrapContextResponseForModifying(HttpContext context, Func<HttpResponse, Task> modifyingInstructions)
        {
            var originBody = context.Response.Body;
            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await modifyingInstructions.Invoke(context.Response);

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originBody);
            context.Response.Body = originBody;
        }

        private string GetTitle(Exception exception)
        {
            return exception switch
            {
                ApiException apie => apie.Message,
                ValidationException ve => "Validation error. Please check your request again.",
                BadHttpRequestException ve => "Bad request made. Please check your request again.",
                _ => "An error occurred and we're working hard to get this working for you again"
            };
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {

                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                BadHttpRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
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
