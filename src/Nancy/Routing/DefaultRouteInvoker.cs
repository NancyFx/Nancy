namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text;
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

            this.invocationStrategies = 
                new Dictionary<Type, Func<dynamic, NancyContext, Response>>
                {
                    { typeof (Response), ProcessAsRealResponse },
                    { typeof (Object), ProcessAsNegotiator },
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

        private Response ProcessAsNegotiator(object routeResult, NancyContext context)
        {
            var negotiator = routeResult as Negotiator;

            if (negotiator == null)
            {
                negotiator = new Negotiator(context);
                negotiator.WithModel(routeResult);
            }

            var acceptHeaders = context.Request.Headers
                .Accept.Where(header => header.Item2 > 0m)
                .Where(header => negotiator.NegotiationContext.PermissableMediaRanges.Any(mr => mr.Matches(header.Item1)))
                .ToList();

            var matches =
                        (from header in acceptHeaders
                         let result = (IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>)GetCompatibleProcessors(header.Item1, negotiator.NegotiationContext.GetModelForMediaRange(header.Item1), context)
                         where result != null
                         select new
                         {
                             header,
                             result
                         }).ToArray();

            if (!matches.Any())
            {
                return new Response();
            }

            var selected = matches.First();

            var processor = selected.result
                .OrderByDescending(x => x.Item2.ModelResult)
                .ThenByDescending(x => x.Item2.RequestedContentTypeResult)
                .First();

            Response response =
                processor.Item1.Process(selected.header.Item1, negotiator.NegotiationContext.GetModelForMediaRange(selected.header.Item1), context);

            var linkProcessors = matches
                .Skip(1)
                .SelectMany(m => m.result)
                .SelectMany(p => p.Item1.ExtensionMappings)
                .ToArray();

            if (linkProcessors.Any())
            {
                response.WithHeader("Vary", "Accept");

                var linkBuilder = new StringBuilder();

                var baseUrl = context.Request.Url.BasePath + "/" + Path.GetFileNameWithoutExtension(context.Request.Url.Path);
                foreach (var linkProcessor in linkProcessors)
                {
                    var url = string.Format("{0}.{1}", baseUrl, linkProcessor.Item1);
                    var contentType = linkProcessor.Item2.ToString();

                    linkBuilder.AppendFormat("<{0}>; rel=\"{1}\",", url, contentType);
                }

                response.Headers["Link"] = linkBuilder.ToString();
            }

            return response;
        }
    }
}