namespace Nancy
{
    /// <summary>
    /// Represents a HEAD only response.
    /// </summary>
	public class HeadResponse : Response
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadResponse"/> class.
        /// </summary>
        /// <param name="response">
        /// The full response to create the head response from.
        /// </param>
        public HeadResponse(Response response)
		{
		    this.Contents = GetStringContents(string.Empty);
			this.ContentType = response.ContentType;
		    this.Headers = response.Headers;
			this.StatusCode = response.StatusCode;
		}
	}
}