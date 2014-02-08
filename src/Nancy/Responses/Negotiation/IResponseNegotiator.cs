namespace Nancy.Responses.Negotiation
{
    /// <summary>
    /// Creates a response from a given result and context.
    /// </summary>
    public interface IResponseNegotiator
    {
        /// <summary>
        /// Negotiates the response based on the given result and context.
        /// </summary>
        /// <param name="routeResult">The route result.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Response"/>.</returns>
        Response NegotiateResponse(dynamic routeResult, NancyContext context);
    }
}