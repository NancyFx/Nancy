namespace Nancy.Responses.Negotiation
{
    using System;

    /// <summary>
    /// Content negotiation response processor
    /// </summary>
    public interface IResponseProcessor
    {
        /// <summary>
        /// Determines whether the the processor can handle a given content type and model
        /// </summary>
        /// <param name="requestedContentType">Content type requested by the client</param>
        /// <param name="model">The model, if any</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A ProcessorMatch result that determines the priority of the processor</returns>
        ProcessorMatch CanProcess(Tuple<string, string> requestedContentType, dynamic model, NancyContext context);

        /// <summary>
        /// Process the response
        /// </summary>
        /// <param name="requestedContentType">Content type requested by the client</param>
        /// <param name="model">The model, if any</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A response</returns>
        Response Process(Tuple<string, string> requestedContentType, dynamic model, NancyContext context);
    }
}