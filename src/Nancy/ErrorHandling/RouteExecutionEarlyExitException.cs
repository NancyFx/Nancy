namespace Nancy.ErrorHandling
{
    using System;

    /// <summary>
    /// Here Be Dragons - Using an exception for flow control to hotwire route execution.
    /// It can be useful to call a method inside a route definition and have that method
    /// immediately return a response (such as for authentication). This exception is used
    /// to allow that flow.
    /// </summary>
    public class RouteExecutionEarlyExitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteExecutionEarlyExitException"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public RouteExecutionEarlyExitException(Response response)
        {
            Response = response;
        }

        /// <summary>
        /// Gets or sets the response
        /// </summary>
        public Response Response { get; protected set; }
    }
}