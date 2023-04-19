using CleanSlice.Api.Common.DTOs;
using CleanSlice.Api.Common.Exceptions;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CleanSlice.Api.Infrastructure.Middleware
{
    internal class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private static async Task HandleException(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);

            ErrorResponse response = new()
            {
                Title = GetTitle(exception),
                Status = statusCode,
                Detail = exception.Message,
                Errors = GetErrors(exception)
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
                NotFoundException nf => nf.Title,
                ValidationException ve => ve.Message,
                _ => "Server Error"
            };
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {

                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
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
