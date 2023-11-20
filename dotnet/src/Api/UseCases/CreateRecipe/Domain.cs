using CleanSlice.Api.Common.Interfaces;
using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe.Domain
{
    public class RecipeBook
    {

        private readonly IQueryable<DataLayer.Models.Recipe> recipes;
        public IList<Recipe> Recipes { get; } = new List<Recipe>();

        public RecipeBook(IQueryable<DataLayer.Models.Recipe> recipes)
        {
            this.recipes = recipes;
        }

        public void AddRecipe(Recipe recipe)
        {
            var validator = new AddRecipeValidator(recipes);
            validator.ValidateAndThrow(recipe);

            Recipes.Add(recipe);
        }
    }

    public class AddRecipeValidator : AbstractValidator<Recipe>, INonInjectableValidator
    {
        private IQueryable<DataLayer.Models.Recipe> recipes;

        public AddRecipeValidator(IQueryable<DataLayer.Models.Recipe> recipes)
        {
            this.recipes = recipes;

            RuleFor(r => r.Title)
                .NotEmpty()
                .Must(title => !this.recipes.Any(r => r.Title.ToUpper() == title.ToUpper()))
                .WithMessage("Recipe with this title already exists. Try updating instead.");
        }

        // Need to add this. For some reason the IoC is still trying to resolve this when we have
        // clearly filter Validators with INonInjectableValidator.
        public AddRecipeValidator()
        {
            this.recipes = Enumerable.Empty<DataLayer.Models.Recipe>().AsQueryable();
        }
    }

    public class Recipe
    {
        public Recipe(string title, IList<Ingredient> ingredients)
        {
            Title = title;
            Ingredients = ingredients;
        }

        public string Title { get; }
        public IList<Ingredient> Ingredients { get; }
    }

    public class Ingredient
    {
        public Ingredient(string name, int amount, string unit)
        {
            Name = name;
            Amount = amount;
            Unit = unit;
        }

        public string Name { get; }
        public int Amount { get; }
        public string Unit { get; }
    }
}
