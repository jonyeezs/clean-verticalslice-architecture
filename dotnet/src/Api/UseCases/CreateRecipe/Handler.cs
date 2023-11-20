using CleanSlice.Api.Common.Interfaces;
using CleanSlice.Api.UseCases.CreateRecipe.Domain;
using MediatR;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Handler : IRequestHandler<CreateRecipeRequest, CreateRecipeResponse>
    {
        private readonly IDbAccess<RecipeBook, IList<(Guid Id, string Title)>> dbAccess;

        public Handler(IDbAccess<RecipeBook, IList<(Guid Id, string Title)>> dbAccess)
        {
            this.dbAccess = dbAccess;
        }
        public async Task<CreateRecipeResponse> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
        {
            var recipeBook = this.dbAccess.Retrieve();
            recipeBook.AddRecipe(new Recipe(request.Title, Array.Empty<Ingredient>()));

            var result = await this.dbAccess.AddAsync(recipeBook, cancellationToken);
            return new CreateRecipeResponse(result.First().Id);
        }
    }
}
