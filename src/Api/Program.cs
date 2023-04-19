using Carter;
using Carter.OpenApi;
using Carter.ResponseNegotiators.Newtonsoft;
using CleanSlice.Api.Infrastructure.Behaviours;
using CleanSlice.Api.Infrastructure.Middleware;
using CleanSlice.Api.Infrastructure.Swagger;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

const string ConnectionsStringName = "Local_DB";


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

string connectionString = builder.Configuration.GetConnectionString(ConnectionsStringName) ?? "";

builder.Services.AddAutoMapper(typeof(Program));

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
