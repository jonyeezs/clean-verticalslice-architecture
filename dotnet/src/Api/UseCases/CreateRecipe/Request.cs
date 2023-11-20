using MediatR;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public record CreateRecipeRequest : IRequest<CreateRecipeResponse>
    {
        public string Url { get; init; } = null!;

        public string Title { get; init; } = null!;

        public string Name { get; init; } = null!;

        public string Author { get; init; } = null!;
    }
}
