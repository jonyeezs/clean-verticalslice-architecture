using CleanSlice.Api.UseCases.CreateRecipe.Domain;
using CleanSlice.Api.Common.Interfaces;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class DbAccess : IDbAccess<RecipeBook, IList<(Guid Id, string Title)>>
    {
        private readonly RecipeContext recipeContext;

        public DbAccess(RecipeContext recipeContext)
        {
            this.recipeContext = recipeContext;
        }

        public RecipeBook Retrieve()
        {
            return new RecipeBook(this.recipeContext.Recipe.AsNoTracking());
        }


        public async Task<IList<(Guid Id, string Title)>> AddAsync(RecipeBook domain, CancellationToken cancellationToken)
        {
            var newRecipes = domain.Recipes
            .Select(r => new DataLayer.Models.Recipe
            {
                Title = r.Title,
                Ingredients = r.Ingredients.Select(i =>
                    new DataLayer.Models.Ingredient { Name = i.Name }).ToArray()
            }).ToList();

            this.recipeContext.Recipe.AddRange(newRecipes);
            await this.recipeContext.SaveChangesAsync(cancellationToken);

            return newRecipes.Select(r => (r.Id, r.Title)).ToArray();
        }
    }
}
