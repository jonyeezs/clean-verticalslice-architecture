namespace CleanSlice.Api.Common.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string title, string message) : base(message)
        {
            Title = title;
        }

        public ApiException()
        {
        }

        public ApiException(string? message) : base(message)
        {
        }

        public ApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public string Title { get; set; } = "An error has occurred";
    }
}
