namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Processes the model for xml media types and extension.
    /// </summary>
    public class XmlProcessor : IResponseProcessor
    {
        private readonly ISerializer serializer;

        private static readonly IEnumerable<Tuple<string, MediaRange>> extensionMappings =
            new[] { new Tuple<string, MediaRange>("xml", new MediaRange("application/xml")) };

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlProcessor"/> class,
        /// with the provided <see paramref="serializers"/>.
        /// </summary>
        /// <param name="serializers">The serializes that the processor will use to process the request.</param>
        public XmlProcessor(IEnumerable<ISerializer> serializers)
        {
            this.serializer = serializers.FirstOrDefault(x => x.CanSerialize("application/xml"));
        }

        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { return extensionMappings; }
        }

        /// <summary>
        /// Determines whether the processor can handle a given content type and model.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="ProcessorMatch"/> result that determines the priority of the processor.</returns>
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (IsExactXmlContentType(requestedMediaRange))
            {
                return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.ExactMatch
                };
            }

            if (IsWildcardXmlContentType(requestedMediaRange))
            {
                return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.NonExactMatch
                };
            }

            return new ProcessorMatch
            {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = MatchResult.NoMatch
            };
        }

        /// <summary>
        /// Process the response.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return CreateResponse(model, serializer);
        }

        private static Response CreateResponse(dynamic model, ISerializer serializer)
        {
            return new Response
            {
                Contents = stream =>
                {
                    if (model != null)
                    {
                        serializer.Serialize("application/xml", model, stream);
                    }
                },
                ContentType = "application/xml",
                StatusCode = HttpStatusCode.OK
            };
        }

        private static bool IsExactXmlContentType(MediaRange requestedContentType)
        {
            if (requestedContentType.Type.IsWildcard && requestedContentType.Subtype.IsWildcard)
            {
                return true;
            }

            return requestedContentType.Matches("application/xml") || requestedContentType.Matches("text/xml");
        }

        private static bool IsWildcardXmlContentType(MediaRange requestedContentType)
        {
            if (!requestedContentType.Type.IsWildcard && !string.Equals("application", requestedContentType.Type, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (requestedContentType.Subtype.IsWildcard)
            {
                return true;
            }

            var subtypeString = requestedContentType.Subtype.ToString();

            return (subtypeString.StartsWith("vnd", StringComparison.OrdinalIgnoreCase) &&
                    subtypeString.EndsWith("+xml", StringComparison.OrdinalIgnoreCase));
        }
    }
}
