using CleanSlice.Api.Common.Interfaces;
using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe.Domain
{
    public class RecipeBook
    {
        private readonly AddRecipeValidator validator;
        public IList<Recipe> Recipes { get; } = new List<Recipe>();

        public RecipeBook(Func<string, CancellationToken, Task<IEnumerable<Recipe>>> recipesOfMatchingTitle)
        {
            this.validator = new AddRecipeValidator(recipesOfMatchingTitle);
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await this.validator.ValidateAndThrowAsync(recipe);

            Recipes.Add(recipe);
        }
    }

    public class AddRecipeValidator : AbstractValidator<Recipe>, INonInjectableValidator
    {
        Func<string, CancellationToken, Task<IEnumerable<Recipe>>> _recipesOfMatchingTitle;
        public AddRecipeValidator(Func<string, CancellationToken, Task<IEnumerable<Recipe>>> recipesOfMatchingTitle)
        {
            _recipesOfMatchingTitle = recipesOfMatchingTitle;

            RuleFor(r => r.Title)
                .NotEmpty()
                .MustAsync(async (title, cancellationToken) =>
                {
                    var recipes = await _recipesOfMatchingTitle.Invoke(title, cancellationToken);
                    return !recipes.Any();
                })
                .WithMessage("Recipe with this title already exists. Try updating instead.");
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
