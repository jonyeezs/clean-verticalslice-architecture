using MediatR;

namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public class Handler : IRequestHandler<CreateRecipeRequest, CreateRecipeResponse>
    {
        public Task<CreateRecipeResponse> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CreateRecipeResponse(5));
        }
    }
}
