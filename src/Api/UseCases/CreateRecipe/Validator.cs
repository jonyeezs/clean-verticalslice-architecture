using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Validator : AbstractValidator<CreateRecipeRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
