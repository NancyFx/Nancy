namespace Nancy.ViewEngines.Razor
{
    using System.Linq;

    /// <summary>
    /// Encapsulates the result of rendering a requested view, allowing the 
    /// <see cref="RazorViewEngine"/> to represent the <see cref="HttpStatusCode"/> 
    /// of the compilation process in the returned <see cref="Response"/> object.
    /// </summary>
    public class NancyRazorBuildViewBodyResult
    {
        /// <summary>
        /// Creates an instance of a <see cref="NancyRazorBuildViewBodyResult"/>.
        /// </summary>
        /// <param name="statusCode">
        /// The <see cref="HttpStatusCode"/> of the server process that rendered the view.
        /// </param>
        /// <param name="body">
        /// The body content of the view after rendering.
        /// </param>
        public NancyRazorBuildViewBodyResult(HttpStatusCode statusCode, dynamic body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        /// <summary>
        /// Indicates the status of the process that built and rendered the view.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// The rendered body of the requested Razor view. (e.g. the resulting HTML).
        /// </summary>
        public dynamic Body { get; private set; }
    }
}