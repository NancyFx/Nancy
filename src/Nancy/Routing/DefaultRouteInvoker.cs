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

        private readonly IDictionary<Type, Func<dynamic, NancyContext, Response>> invocationStrategies; 

        public DefaultRouteInvoker(IEnumerable<IResponseProcessor> processors)
        {
            this.processors = processors;

            this.invocationStrategies = new Dictionary<Type, Func<dynamic, NancyContext, Response>>
                                            {
                                                { typeof (Response), ProcessAsRealResponse },
                                                { typeof (Negotiator), ProcessAsNegotiator },
                                                { typeof (Object), ProcessAsModel}
                                            };
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
                route.Invoke(parameters) ?? new Response();

            var strategy = this.GetInvocationStrategy(result.GetType());

            return strategy.Invoke(result, context);
        }

        private Func<dynamic, NancyContext, Response> GetInvocationStrategy(Type resultType)
        {
            return invocationStrategies.Where(invocationStrategy => invocationStrategy.Key.IsAssignableFrom(resultType))
                                        .Select(invocationStrategy => invocationStrategy.Value)
                                        .First();
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

        private Response ProcessAsRealResponse(dynamic routeResult, NancyContext context)
        {
            return (Response)routeResult;
        }

        private Response ProcessAsNegotiator(dynamic routeResult, NancyContext context)
        {
            // TODO - ignore any processors that don't fit the allowed list (using GetFullOutputContentType)
            // TODO - for the best matching processor, get the return content type and either use a specific model or the default model
            throw new NotImplementedException();
        }

        private Response ProcessAsModel(dynamic model, NancyContext context)
        {
            var acceptHeaders =
                context.Request.Headers.Accept.Where(header => header.Item2 > 0m).ToList();

            var matches =
                (from header in acceptHeaders
                let result = (IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>)GetCompatibleProcessors(header.Item1, model, context)
                where result != null
                select new
                {
                    header,
                    result
                }).ToArray();

            if (matches.Any())
            {
                var selected = matches.First();

                var processor = selected.result
                    .OrderByDescending(x => x.Item2.ModelResult)
                    .ThenByDescending(x => x.Item2.RequestedContentTypeResult)
                    .First();

                var response =
                    processor.Item1.Process(selected.header.Item1, model, context);

                if (matches.Count() > 1)
                {
                    ((Response)response).WithHeader("Vary", "Accept");
                }

                return response;
            }

            // What do we return if nothing could process it?
            return new Response();
        }
    }

    //public class FakeResponseProcessor : IResponseProcessor
    //{
    //    public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
    //    {
    //        return requestedMediaRange.Subtype.Equals("xml") ?
    //            new ProcessorMatch { ModelResult = MatchResult.NoMatch, RequestedContentTypeResult = MatchResult.NoMatch } :
    //            new ProcessorMatch{ ModelResult = MatchResult.ExactMatch, RequestedContentTypeResult = MatchResult.ExactMatch };
    //    }

    //    public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
    //    {
    //        return new Response();
    //    }
    //}
}