using CleanSlice.Api.Common.Interfaces;
using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe.Domain
{
    public class RecipeBook
    {
        private readonly AddRecipeValidator validator;
        public IList<Recipe> Recipes { get; } = new List<Recipe>();

        public RecipeBook(Func<string, IEnumerable<Recipe>> getRecipesByTitle)
        {
            this.validator = new AddRecipeValidator(getRecipesByTitle);
        }

        public void AddRecipe(Recipe recipe)
        {
            this.validator.ValidateAndThrow(recipe);

            Recipes.Add(recipe);
        }
    }

    public class AddRecipeValidator : AbstractValidator<Recipe>, INonInjectableValidator
    {
        private Func<string, IEnumerable<Recipe>> getRecipesByTitle;

        public AddRecipeValidator(Func<string, IEnumerable<Recipe>> getRecipesByTitle)
        {
            this.getRecipesByTitle = getRecipesByTitle;

            RuleFor(r => r.Title)
                .NotEmpty()
                .Must(title => !this.getRecipesByTitle(title).Any())
                .WithMessage("Recipe with this title already exists. Try updating instead.");
        }

        // Need to add this. For some reason the IoC is still trying to resolve this when we have
        // clearly filter Validators with INonInjectableValidator.
        public AddRecipeValidator()
        {
            this.getRecipesByTitle = (_) => Enumerable.Empty<Recipe>();
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
