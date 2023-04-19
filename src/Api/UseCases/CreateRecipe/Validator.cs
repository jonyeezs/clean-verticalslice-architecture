using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Validator : AbstractValidator<CreateRecipeRequest>
    {
        public Validator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}
