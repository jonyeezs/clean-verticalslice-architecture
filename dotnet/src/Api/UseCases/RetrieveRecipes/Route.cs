using Carter;
using Carter.OpenApi;
using CleanSlice.Api.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public class Route : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            _ = app.MapGet(DomainRoutes.RECIPE, async ([AsParameters] RetrieveRecipeParameters request, CancellationToken cancellationToken, IMediator mediator)
                => await mediator.Send(request, cancellationToken))
                .AllowAnonymous()
                .IncludeInOpenApi()
                .WithTags(DomainRoutes.RECIPE)
                .WithDescription("Retrieve recipe by optional parameters")
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        }
    }
}
