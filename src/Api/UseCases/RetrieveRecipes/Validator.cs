using FluentValidation;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public class Validator : AbstractValidator<RetrieveRecipeParameters>
    {
        public Validator()
        {
            _ = RuleFor(x => x.Id).NotNull().Must(value => value != 0);
        }
    }
}
