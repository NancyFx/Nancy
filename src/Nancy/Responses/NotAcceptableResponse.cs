namespace Nancy.Responses
{
    /// <summary>
    /// Response with status code 406 (Not Acceptable).
    /// </summary>
    public class NotAcceptableResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAcceptableResponse"/> class.
        /// </summary>
        public NotAcceptableResponse()
        {
            this.StatusCode = HttpStatusCode.NotAcceptable;
        }
    }
}