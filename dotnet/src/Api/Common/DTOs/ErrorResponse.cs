namespace CleanSlice.Api.Common.DTOs
{
    public class ErrorResponse
    {
        public string Title { get; set; } = "An error has occurred";
        public int Status { get; set; }
        public string? Detail { get; set; }
        public IReadOnlyDictionary<string, string[]> Errors { get; set; } = null!;
    }
}
