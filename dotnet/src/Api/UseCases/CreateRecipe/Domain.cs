using CleanSlice.Api.Common.Attributes;
using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe.Domain
{
    public class RecipeBook(Func<string, CancellationToken, Task<IEnumerable<Recipe>>> recipesOfMatchingTitle)
    {
        private readonly AddRecipeValidator validator = new(recipesOfMatchingTitle);
        public IList<Recipe> Recipes { get; } = [];

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await this.validator.ValidateAndThrowAsync(recipe);

            Recipes.Add(recipe);
        }
    }

    [NonInjectableValidator]
    public class AddRecipeValidator : AbstractValidator<Recipe>
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

    public class Recipe(string title, IList<Ingredient> ingredients)
    {
        public string Title => title;
        public IList<Ingredient> Ingredients => ingredients;
    }

    public class Ingredient(string name, int amount, string unit)
    {
        public string Name => name;
        public int Amount => amount;
        public string Unit => unit;
    }
}
