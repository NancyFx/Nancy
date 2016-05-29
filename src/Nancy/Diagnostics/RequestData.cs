namespace Nancy.Diagnostics
{
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Stores request trace information about the request.
    /// </summary>
    public class RequestData
    {
        /// <summary>
        /// Gets or sets the content type of the request.
        /// </summary>
        /// <value>A <see cref="MediaRange"/> containing the content type.</value>
        public MediaRange ContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers of the request.
        /// </summary>
        /// <value>A <see cref="RequestHeaders"/> instance containing the headers.</value>
        public RequestHeaders Headers { get; set; }

        /// <summary>
        /// Gets the HTTP verb of the request.
        /// </summary>
        /// <value>A <see cref="string"/> containing the HTTP verb.</value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Url"/> that was requested.
        /// </summary>
        public Url Url { get; set; }

        /// <summary>
        /// Implicitly casts a <see cref="Request"/> instance into a <see cref="RequestData"/> instance.
        /// </summary>
        /// <param name="request">A <see cref="Request"/> instance.</param>
        /// <returns>A <see cref="RequestData"/> instance.</returns>
        public static implicit operator RequestData(Request request)
        {
            return new RequestData
            {
                ContentType = request.Headers.ContentType,
                Headers = request.Headers,
                Method = request.Method,
                Url = request.Url
            };
        }
    }
}