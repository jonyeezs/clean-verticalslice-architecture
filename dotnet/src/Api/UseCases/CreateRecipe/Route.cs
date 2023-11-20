using Carter;
using Carter.OpenApi;
using CleanSlice.Api.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Route : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            _ = app.MapPost(DomainRoutes.RECIPE, async ([FromBody] CreateRecipeRequest request, CancellationToken cancellationToken, IMediator mediator)
                =>
            {
                CreateRecipeResponse response = await mediator.Send(request, cancellationToken);

                return Results.Created($"/recipes/{response.Id}", response);
            })
                .AllowAnonymous()
                .IncludeInOpenApi()
                .WithTags(DomainRoutes.RECIPE)
                .WithDescription("Create a recipe. Responses with its allocated identifier")
                .Produces<CreateRecipeResponse>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        }
    }
}
