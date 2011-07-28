namespace Nancy.ErrorHandling
{
    /// <summary>
    /// Provides informative responses for particular HTTP status codes
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Whether then 
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <returns>True if handled, false otherwise</returns>
        bool HandlesStatusCode(HttpStatusCode statusCode);

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">Current context</param>
        void Handle(HttpStatusCode statusCode, NancyContext context);
    }
}