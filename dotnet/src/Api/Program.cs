using System.Reflection;
using Carter;
using Carter.OpenApi;
using Carter.ResponseNegotiators.Newtonsoft;
using CleanSlice.Api.Common.Interfaces;
using CleanSlice.Api.Infrastructure.Behaviours;
using CleanSlice.Api.Infrastructure.Middleware;
using CleanSlice.Api.Infrastructure.Swagger;
using DataLayer;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

const string RecipeConnectionStringSettingName = "Recipe";


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeContext>((opts) => opts.UseNpgsql(builder.Configuration.GetConnectionString(RecipeConnectionStringSettingName)));

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
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

// Add all implementation of available IDbAccess
foreach (var t in Assembly.GetExecutingAssembly().GetTypes()
    .Where(c => !c.IsInterface
        && c.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDbAccess<,>))))
{
    var genericInterface = t.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IDbAccess<,>));
    builder.Services.AddScoped(genericInterface, t);
}


builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped,
    (a) => !typeof(INonInjectableValidator).IsAssignableFrom(a.ValidatorType));

//
WebApplication app = builder.Build();

app.MapHealthChecks("/health");

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}


app.UseHttpsRedirection();
app.MapCarter();
app.Run();
