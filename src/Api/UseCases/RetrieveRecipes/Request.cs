using MediatR;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public record RetrieveRecipeParameters : IRequest<RetrieveRecipesResponse>
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Author { get; set; }
    }
}
