namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public record CreateRecipeResponse
    {
        public Guid Id { get; set; }

        public CreateRecipeResponse(Guid id)
        {
            Id = id;
        }
    }
}
