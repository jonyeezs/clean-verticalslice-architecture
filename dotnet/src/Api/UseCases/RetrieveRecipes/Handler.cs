using CleanSlice.Api.Common.Exceptions;
using DataLayer;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public class Handler : IRequestHandler<RetrieveRecipeParameters, RetrieveRecipesResponse>
    {
        private readonly RecipeContext recipeContext;
        public Handler(RecipeContext recipeContext)
        {
            this.recipeContext = recipeContext;

        }
        public async Task<RetrieveRecipesResponse> Handle(RetrieveRecipeParameters request, CancellationToken cancellationToken)
        {
            var recipe = await this.recipeContext.Recipe
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                (request.Id != null && request.Id == r.Id) ||
                (request.Name != null && request.Name.ToUpper() == r.Title.ToUpper()), cancellationToken);

            if (recipe == null)
            {
                throw new NotFoundException("Recipe not found");
            }

            return new RetrieveRecipesResponse("", recipe.Title);
        }
    }
}
