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
        /// Determines whether the processor can handle a given content type and model.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="ProcessorMatch"/> result that determines the priority of the processor.</returns>
        ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context);

        /// <summary>
        /// Process the response.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context);
    }
}