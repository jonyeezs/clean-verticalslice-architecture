using FluentValidation;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Validator : AbstractValidator<CreateRecipeRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.Url))
                .WithMessage("Url ({PropertyValue}) is not valid");
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
