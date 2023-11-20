using MediatR;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public record RetrieveRecipeParameters : IRequest<RetrieveRecipesResponse>
    {
        public Guid? Id { get; init; }

        public string? Name { get; init; }
    }
}
