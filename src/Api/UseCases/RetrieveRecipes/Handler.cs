using CleanSlice.Api.Common.Exceptions;
using MediatR;

namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public class Handler : IRequestHandler<RetrieveRecipeParameters, RetrieveRecipesResponse>
    {
        public Task<RetrieveRecipesResponse> Handle(RetrieveRecipeParameters request, CancellationToken cancellationToken)
        {
            return request.Author == "James"
                ? Task.FromResult(new RetrieveRecipesResponse("http://google.com", "Dunkin", "James Peach"))
                : throw new NotFoundException("Unable to find any recipe",
                    $"The author {request.Author} does not have any recipe");
        }
    }
}
