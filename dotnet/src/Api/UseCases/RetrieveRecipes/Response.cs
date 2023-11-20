namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public record RetrieveRecipesResponse
    {
        public string Url { get; init; }

        public string Title { get; init; }

        public RetrieveRecipesResponse(string url, string title)
        {
            Url = url;
            Title = title;
        }
    }
}
