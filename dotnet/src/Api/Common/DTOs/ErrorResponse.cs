namespace CleanSlice.Api.Common.DTOs
{
    public record ErrorResponse(
        string Title = "An error has occurred",
        IReadOnlyDictionary<string, string[]>? Details = null);
}
