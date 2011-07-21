namespace Nancy.Responses
{
    /// <summary>
    /// A response representing a raw 303 redirect
    /// <seealso cref="Nancy.Extensions.ContextExtensions.ToFullPath"/>
    /// <seealso cref="Nancy.Extensions.ContextExtensions.GetRedirect"/>
    /// </summary>
    public class RedirectResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResponse"/> class. 
        /// </summary>
        /// <param name="location">
        /// Location to redirect to
        /// </param>
        public RedirectResponse(string location)
        {
            this.Headers.Add("Location", location);
            this.Contents = GetStringContents(string.Empty);
            this.ContentType = "text/html";
            this.StatusCode = HttpStatusCode.SeeOther;
        }
    }
}