using System.Data;
using FluentValidation;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public class Validator : AbstractValidator<RetrieveRecipeParameters>
    {
        public Validator()
        {
            const string optionError = "At least provide the recipe's Id or the recipe name is required";

            RuleFor(x => x.Id).NotEmpty().When(r => string.IsNullOrEmpty(r.Name))
                .WithMessage(optionError);
            RuleFor(x => x.Name).NotEmpty().When(r => r.Id == null || r.Id == Guid.Empty)
                .WithMessage(optionError); ;
        }
    }
}
