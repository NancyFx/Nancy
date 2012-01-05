namespace Nancy
{
    using System;

    /// <summary>
    /// Exception that is thrown when an unhandled exception occured during
    /// the execution of the current request.
    /// </summary>
    public class RequestExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException"/>, with
        /// the specified <paramref name="innerException"/>.
        /// </summary>
        /// <param name="innerException"></param>
        public RequestExecutionException(Exception innerException)
            : base("Oh noes!", innerException)
        {
        }
    }
}