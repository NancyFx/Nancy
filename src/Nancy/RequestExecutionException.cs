namespace Nancy
{
    using System;

    public class RequestExecutionException : Exception
    {
        public RequestExecutionException(Exception innerException)
            : base("Oh noes!", innerException)
        {
        }
    }
}