namespace CleanSlice.Api.UseCases.CreateRecipe
{
    public record CreateRecipeResponse
    {
        public int Id { get; set; }

        public CreateRecipeResponse(int id)
        {
            Id = id;
        }
    }
}
