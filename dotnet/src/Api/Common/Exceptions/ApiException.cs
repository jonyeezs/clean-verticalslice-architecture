namespace CleanSlice.Api.Common.Exceptions
{
    public abstract class ApiException : Exception
    {
        public ApiException(string message = "An error has occurred") : base(message)
        {
        }

        public ApiException()
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
