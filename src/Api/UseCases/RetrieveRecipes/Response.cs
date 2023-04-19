namespace CleanSlice.Api.UseCases.RetrieveRecipes
{
    public record RetrieveRecipesResponse
    {
        public string Url { get; init; }

        public string Title { get; init; }

        public string Author { get; init; }

        public RetrieveRecipesResponse(string url, string title, string author)
        {
            Url = url;
            Title = title;
            Author = author;
        }
    }
}
