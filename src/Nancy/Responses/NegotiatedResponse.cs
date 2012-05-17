namespace Nancy.Responses
{
    /// <summary>
    /// Response that indicates that the response format should be negotiated between the client and the server.
    /// </summary>
    public class NegotiatedResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NegotiatedResponse"/> response for the
        /// provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The response value that should be negotiated.</param>
        public NegotiatedResponse(dynamic value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value that should be negotiated.
        /// </summary>
        public dynamic Value { get; set; }
    }
}