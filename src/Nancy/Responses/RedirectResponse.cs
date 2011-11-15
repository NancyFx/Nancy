namespace Nancy.Responses
{
    /// <summary>
    /// A response representing an HTTP redirect
    /// <seealso cref="Nancy.Extensions.ContextExtensions.ToFullPath"/>
    /// <seealso cref="Nancy.Extensions.ContextExtensions.GetRedirect"/>
    /// </summary>
    public class RedirectResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResponse"/> class. 
        /// </summary>
        /// <param name="location">Location to redirect to</param>
        /// <param name="type">Type of redirection to perform</param>
        public RedirectResponse(string location, RedirectType type = RedirectType.SeeOther)
        {
            this.Headers.Add("Location", location);
            this.Contents = GetStringContents(string.Empty);
            this.ContentType = "text/html";
            switch (type)
            {
                case RedirectType.Permanent:
                    this.StatusCode = HttpStatusCode.MovedPermanently;
                    break;
                case RedirectType.Temporary:
                    this.StatusCode = HttpStatusCode.TemporaryRedirect;
                    break;
                default:
                    this.StatusCode = HttpStatusCode.SeeOther;
                    break;
            }
        }

        /// <summary>
        /// Which type of redirect
        /// </summary>
        public enum RedirectType
        {
            /// <summary>
            /// HTTP 301 - All future requests should be to this URL
            /// </summary>
            Permanent,
            /// <summary>
            /// HTTP 307 - Redirect this request but allow future requests to the original URL
            /// </summary>
            Temporary,
            /// <summary>
            /// HTTP 303 - Redirect this request using an HTTP GET
            /// </summary>
            SeeOther
        }
    }
}