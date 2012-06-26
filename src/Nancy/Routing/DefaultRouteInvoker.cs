namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Responses.Negotiation;

    /// <summary>
    /// Default route invoker implementation-
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        private readonly IEnumerable<IResponseProcessor> processors;

        public DefaultRouteInvoker(IEnumerable<IResponseProcessor> processors)
        {
            this.processors = processors;
        }

        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <param name="context">The context of the route that is being invoked.</param>
        /// <returns>A <see cref="Response"/> intance that represents the result of the invoked route.</returns>
        public Response Invoke(Route route, DynamicDictionary parameters, NancyContext context)
        {
            var result =
                route.Invoke(parameters);

            var response =
                CastResultToResponse(result) ?? GetNegotiatedResponse(result, context);

            return response;
        }

        private static Response CastResultToResponse(dynamic result)
        {
            return result as Response;
        }

        private Response GetNegotiatedResponse(dynamic model, NancyContext context)
        {
            var acceptHeaders =
                context.Request.Headers.Accept.ToList();

            foreach (var header in acceptHeaders)
            {
                var match = (IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>)GetCompatibleProcessors(header.Item1, model, context);

                if (match == null)
                {
                    continue;
                }

                var prioritized = match
                    .OrderByDescending(x => x.Item2.ModelResult)
                    .ThenByDescending(x => x.Item2.RequestedContentTypeResult)
                    .First();

                return prioritized.Item1.Process(header.Item1, model, context);
            }

            // What do we return if nothing could process it?
            return new Response();
        }

        private IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> GetCompatibleProcessors(string acceptHeader, dynamic model, NancyContext context)
        {
            var compatibleProcessors = this.processors
                .Select(processor => Tuple.Create(processor, (ProcessorMatch)processor.CanProcess(acceptHeader, model, context)))
                .Where(x => x.Item2.ModelResult != MatchResult.NoMatch)
                .Where(x => x.Item2.RequestedContentTypeResult != MatchResult.NoMatch)
                .ToList();

            return compatibleProcessors.Any() ?
                compatibleProcessors :
                null;
        }
    }

    public class FakeResponseProcessor : IResponseProcessor
    {
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { throw new NotImplementedException(); }
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return requestedMediaRange.Subtype.Equals("xml") ?
                new ProcessorMatch { ModelResult = MatchResult.NoMatch, RequestedContentTypeResult = MatchResult.NoMatch } :
                new ProcessorMatch{ ModelResult = MatchResult.ExactMatch, RequestedContentTypeResult = MatchResult.ExactMatch };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}