namespace Nancy
{
    /// <summary>
    /// Not Found response
    /// </summary>
    /// <seealso cref="Nancy.Response" />
    public class NotFoundResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundResponse"/> class.
        /// </summary>
        public NotFoundResponse()
        {
            this.ContentType = "text/html";
            this.StatusCode = HttpStatusCode.NotFound;
        }
    }
}