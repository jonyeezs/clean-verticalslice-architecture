using CleanSlice.Api.UseCases.CreateRecipe.Domain;
using CleanSlice.Api.Common.Interfaces;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class DataAccess : IDataAccess<RecipeBook, IList<(Guid Id, string Title)>>
    {
        private readonly RecipeContext recipeContext;

        public DataAccess(RecipeContext recipeContext)
        {
            this.recipeContext = recipeContext;
        }

        public RecipeBook Retrieve()
        {
            return new RecipeBook(this.FindRecipeByTitle);
        }


        public async Task<IList<(Guid Id, string Title)>> SaveAsync(RecipeBook domain, CancellationToken cancellationToken)
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

        private IEnumerable<Recipe> FindRecipeByTitle(string title)
        {
            return this.recipeContext.Recipe
            .AsNoTracking()
            .Where(r => r.Title.ToLower().Equals(title.ToLower()))
            .Select(r => new Recipe(r.Title, r.Ingredients.Select(i => new Ingredient(i.Name, 0, "gm")).ToList()))
            .AsEnumerable<Recipe>();
        }
    }
}
