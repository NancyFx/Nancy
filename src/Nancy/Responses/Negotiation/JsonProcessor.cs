namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class JsonProcessor : IResponseProcessor
    {
        private readonly ISerializer serializer;

        public JsonProcessor(IEnumerable<ISerializer> serializers)
        {
            this.serializer = serializers.FirstOrDefault(x => x.CanSerialize("application/json"));
        }

        private static IEnumerable<Tuple<string, MediaRange>> extensionMappings = 
            new[] { new Tuple<string, MediaRange>("json", MediaRange.FromString("application/json")) };

        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get
            {
                return extensionMappings;
            }
        }

        /// <summary>
        /// Determines whether the the processor can handle a given content type and model
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client</param>
        /// <param name="model">The model for the given media range</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A ProcessorMatch result that determines the priority of the processor</returns>
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (this.IsExactJsonContentType(requestedMediaRange))
            {
                return new ProcessorMatch
                    {
                        ModelResult = MatchResult.DontCare,
                        RequestedContentTypeResult = MatchResult.ExactMatch
                    };
            }

            if (this.IsWildcardJsonContentType(requestedMediaRange))
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
        /// Process the response
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client</param>
        /// <param name="model">The model for the given media range</param>
        /// <param name="context">The nancy context</param>
        /// <returns>A response</returns>
        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new JsonResponse(model, this.serializer);
        }

        private bool IsExactJsonContentType(MediaRange requestedContentType)
        {
            if (requestedContentType.Type.IsWildcard && requestedContentType.Subtype.IsWildcard)
            {
                return true;
            }

            return requestedContentType.Matches("application/json") || requestedContentType.Matches("text/json");
        }

        private bool IsWildcardJsonContentType(MediaRange requestedContentType)
        {
            if (!requestedContentType.Type.IsWildcard && !string.Equals("application", requestedContentType.Type, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (requestedContentType.Subtype.IsWildcard)
            {
                return true;
            }

            var subtypeString = requestedContentType.Subtype.ToString();

            return (subtypeString.StartsWith("vnd", StringComparison.InvariantCultureIgnoreCase) &&
                    subtypeString.EndsWith("+json", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}