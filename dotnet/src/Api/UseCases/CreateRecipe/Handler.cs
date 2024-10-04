using CleanSlice.Api.Common.Interfaces;
using CleanSlice.Api.UseCases.CreateRecipe.Domain;
using MediatR;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Handler : IRequestHandler<CreateRecipeRequest, CreateRecipeResponse>
    {
        private readonly IDataAccess<CreateRecipeRequest, RecipeBook, IList<(Guid Id, string Title)>> dataAccess;

        public Handler(IDataAccess<CreateRecipeRequest, RecipeBook, IList<(Guid Id, string Title)>> dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        public async Task<CreateRecipeResponse> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
        {
            var recipeBook = await this.dataAccess.RetrieveAsync(request, cancellationToken);
            await recipeBook.AddRecipeAsync(new Recipe(request.Title, Array.Empty<Ingredient>()));

            var result = await this.dataAccess.SaveAsync(recipeBook, cancellationToken);
            return new CreateRecipeResponse(result.First().Id);
        }
    }
}
