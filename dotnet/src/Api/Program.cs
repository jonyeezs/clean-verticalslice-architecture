using System.Reflection;
using Carter;
using Carter.OpenApi;
using Carter.ResponseNegotiators.Newtonsoft;
using CleanSlice.Api.Common.Attributes;
using CleanSlice.Api.Common.Interfaces;
using CleanSlice.Api.Infrastructure.Behaviours;
using CleanSlice.Api.Infrastructure.Middleware;
using CleanSlice.Api.Infrastructure.Swagger;
using DataLayer;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeContext>((opts) => opts.UseNpgsql(builder.Configuration.GetConnectionString("Recipe")));

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    c.DocInclusionPredicate((s, description) =>
    {
        foreach (object metaData in description.ActionDescriptor.EndpointMetadata)
        {
            if (metaData is IIncludeOpenApi)
            {
                return true;
            }
        }
        return false;
    });
    c.DocumentFilter<JsonPatchDocumentFilter>();
});

builder.Services.AddCarter(configurator: c => _ = c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>());

builder.Services.AddMediatR(cfg =>
{
    _ = cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    _ = cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    _ = cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    _ = cfg.AddOpenBehavior(typeof(TransactionBehaviour<,>));
});

// Add all implementation of available IDataAccess
foreach (var t in Assembly.GetExecutingAssembly().GetTypes()
    .Where(c => c.IsClass
        && c.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDataAccess<,,>))))
{
    var genericInterface = t.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IDataAccess<,,>));
    builder.Services.AddScoped(genericInterface, t);
}

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped,
    (filter) => filter.ValidatorType.GetCustomAttributes(typeof(NonInjectableValidatorAttribute), true).Length == 0);

WebApplication app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}


app.UseHttpsRedirection();
app.MapCarter();
app.Run();
