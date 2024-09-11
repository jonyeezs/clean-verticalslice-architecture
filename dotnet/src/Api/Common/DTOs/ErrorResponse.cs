namespace CleanSlice.Api.Common.DTOs
{
    public record ErrorResponse(
        string Title = "An error has occurred",
        string? Detail = null,
        IReadOnlyDictionary<string, string[]>? Errors = null);
}
