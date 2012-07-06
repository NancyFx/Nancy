namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Content negotiation response processor
    /// </summary>
    public interface IResponseProcessor
    {
        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        IEnumerable<Tuple<string, MediaRange>> ExtensionMappings { get; }

        /// <summary>
        /// Returns the full (non-wildcard) content type that this processor will
        /// return for the given media range, model and context.
        /// A call to this is only valid if the processor has previously reported that
        /// it can process the given range, model and context.
        /// </summary>
        /// <param name="requestedMediaRange">Media range requested</param>
        /// <param name="context">Context</param>
        /// <returns>Non-wildcard content type in the form A/B</returns>
        string GetFullOutputContentType(MediaRange requestedMediaRange, NancyContext context);

        /// <summary>
        /// Determines whether the the processor can handle a given content type and model
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A ProcessorMatch result that determines the priority of the processor</returns>
        ProcessorMatch CanProcess(MediaRange requestedMediaRange, NancyContext context);

        /// <summary>
        /// Process the response
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A response</returns>
        Response Process(MediaRange requestedMediaRange, NancyContext context);
    }
}