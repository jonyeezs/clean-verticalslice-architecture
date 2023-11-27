using CleanSlice.Api.Common.Interfaces;
using CleanSlice.Api.UseCases.CreateRecipe.Domain;
using MediatR;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Handler : IRequestHandler<CreateRecipeRequest, CreateRecipeResponse>
    {
        private readonly IDataAccess<RecipeBook, IList<(Guid Id, string Title)>> dataAccess;

        public Handler(IDataAccess<RecipeBook, IList<(Guid Id, string Title)>> dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        public async Task<CreateRecipeResponse> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
        {
            var recipeBook = this.dataAccess.Retrieve();
            recipeBook.AddRecipe(new Recipe(request.Title, Array.Empty<Ingredient>()));

            var result = await this.dataAccess.AddAsync(recipeBook, cancellationToken);
            return new CreateRecipeResponse(result.First().Id);
        }
    }
}
