namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Nancy.Conventions;
    using Nancy.Extensions;
    using Nancy.Helpers;

    /// <summary>
    /// The default implementation for a response negotiator.
    /// </summary>
    public class DefaultResponseNegotiator : IResponseNegotiator
    {
        private readonly IReadOnlyCollection<IResponseProcessor> processors;
        private readonly AcceptHeaderCoercionConventions coercionConventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseNegotiator"/> class.
        /// </summary>
        /// <param name="processors">The response processors.</param>
        /// <param name="coercionConventions">The Accept header coercion conventions.</param>
        public DefaultResponseNegotiator(IEnumerable<IResponseProcessor> processors, AcceptHeaderCoercionConventions coercionConventions)
        {
            this.processors = processors.ToArray();
            this.coercionConventions = coercionConventions;
        }

        /// <summary>
        /// Negotiates the response based on the given result and context.
        /// </summary>
        /// <param name="routeResult">The route result.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Response" />.</returns>
        public Response NegotiateResponse(dynamic routeResult, NancyContext context)
        {
            Response response;
            if (TryCastResultToResponse(routeResult, out response))
            {
                context.WriteTraceLog(sb =>
                    sb.AppendLine("[DefaultResponseNegotiator] Processing as real response"));

                return response;
            }

            context.WriteTraceLog(sb =>
                sb.AppendLine("[DefaultResponseNegotiator] Processing as negotiation"));

            NegotiationContext negotiationContext = GetNegotiationContext(routeResult, context);

            var coercedAcceptHeaders = this.GetCoercedAcceptHeaders(context).ToArray();

            context.WriteTraceLog(sb => GetAccepHeaderTraceLog(context, negotiationContext, coercedAcceptHeaders, sb));

            var compatibleHeaders = this.GetCompatibleHeaders(coercedAcceptHeaders, negotiationContext, context).ToArray();

            if (!compatibleHeaders.Any())
            {
                context.WriteTraceLog(sb =>
                    sb.AppendLine("[DefaultResponseNegotiator] Unable to negotiate response - no headers compatible"));

                return new NotAcceptableResponse();
            }

            return CreateResponse(compatibleHeaders, negotiationContext, context);
        }

        /// <summary>
        /// Tries to cast the dynamic result to a <see cref="Response"/>.
        /// </summary>
        /// <param name="routeResult">The result.</param>
        /// <param name="response">The response.</param>
        /// <returns><c>true</c> if the result is a <see cref="Response"/>, <c>false</c> otherwise.</returns>
        private static bool TryCastResultToResponse(dynamic routeResult, out Response response)
        {
            var targetType = routeResult.GetType();
            var responseType = typeof(Response);

            if (routeResult is Response)
            {
                response = (Response)routeResult;
                return true;
            }

            var methods = responseType.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (var method in methods)
            {
                if (!method.Name.Equals("op_Implicit", StringComparison.Ordinal))
                {
                    continue;
                }

                if (method.ReturnType != responseType)
                {
                    continue;
                }

                var parameters = method.GetParameters();

                if (parameters.Length != 1)
                {
                    continue;
                }

                if (parameters[0].ParameterType != targetType)
                {
                    continue;
                }

                response = (Response)routeResult;
                return true;
            }

            response = null;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="NegotiationContext"/> based on the given result and context.
        /// </summary>
        /// <param name="routeResult">The route result.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="NegotiationContext"/>.</returns>
        private static NegotiationContext GetNegotiationContext(object routeResult, NancyContext context)
        {
            var negotiator = routeResult as Negotiator;

            if (negotiator == null)
            {
                context.WriteTraceLog(sb =>
                    sb.AppendFormat("[DefaultResponseNegotiator] Wrapping result of type {0} in negotiator\n", routeResult.GetType()));

                negotiator = new Negotiator(context).WithModel(routeResult);
            }

            return negotiator.NegotiationContext;
        }

        /// <summary>
        /// Gets the coerced accept headers based on the <see cref="AcceptHeaderCoercionConventions"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable{Tuple{System.String, System.Decimal}}.</returns>
        private IEnumerable<Tuple<string, decimal>> GetCoercedAcceptHeaders(NancyContext context)
        {
            return this.coercionConventions.Aggregate(context.Request.Headers.Accept, (current, coercion) => coercion.Invoke(current, context));
        }

        private static void GetAccepHeaderTraceLog(
            NancyContext context,
            NegotiationContext negotiationContext,
            Tuple<string, decimal>[] coercedAcceptHeaders,
            StringBuilder sb)
        {
            var allowableFormats = negotiationContext.PermissableMediaRanges
                .Select(mr => mr.ToString())
                .Aggregate((t1, t2) => t1 + ", " + t2);

            var originalAccept = context.Request.Headers["accept"].Any()
                ? string.Join(", ", context.Request.Headers["accept"])
                : "None";

            var coercedAccept = coercedAcceptHeaders.Any()
                ? coercedAcceptHeaders.Select(h => h.Item1).Aggregate((t1, t2) => t1 + ", " + t2)
                : "None";

            sb.AppendFormat("[DefaultResponseNegotiator] Original accept header: {0}\n", originalAccept);
            sb.AppendFormat("[DefaultResponseNegotiator] Coerced accept header: {0}\n", coercedAccept);
            sb.AppendFormat("[DefaultResponseNegotiator] Acceptable media ranges: {0}\n", allowableFormats);
        }

        private IEnumerable<CompatibleHeader> GetCompatibleHeaders(
            IEnumerable<Tuple<string, decimal>> coercedAcceptHeaders,
            NegotiationContext negotiationContext,
            NancyContext context)
        {
            var acceptHeaders = GetCompatibleHeaders(coercedAcceptHeaders, negotiationContext);

            foreach (var header in acceptHeaders)
            {
                var mediaRangeModel = negotiationContext.GetModelForMediaRange(header.Item1);

                IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> compatibleProcessors =
                    this.GetCompatibleProcessorsByHeader(header.Item1, mediaRangeModel, context);

                if (compatibleProcessors.Any())
                {
                    yield return new CompatibleHeader(header.Item1, compatibleProcessors);
                }
            }
        }

        private static IEnumerable<Tuple<string, decimal>> GetCompatibleHeaders(
            IEnumerable<Tuple<string, decimal>> coercedAcceptHeaders,
            NegotiationContext negotiationContext)
        {
            var permissableMediaRanges = negotiationContext.PermissableMediaRanges;
            if (permissableMediaRanges.Any(mr => mr.IsWildcard))
            {
                return coercedAcceptHeaders.Where(header => header.Item2 > 0m);
            }

            return coercedAcceptHeaders
                .Where(header => header.Item2 > 0m)
                .SelectMany(header => permissableMediaRanges
                    .Where(mr => mr.Matches(header.Item1))
                    .Select(mr => Tuple.Create(mr.ToString(), header.Item2)));
        }

        /// <summary>
        /// Gets compatible response processors by header.
        /// </summary>
        /// <param name="acceptHeader">The accept header.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable{Tuple{IResponseProcessor, ProcessorMatch}}.</returns>
        private IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> GetCompatibleProcessorsByHeader(
            string acceptHeader, dynamic model, NancyContext context)
        {
            foreach (var processor in this.processors)
            {
                ProcessorMatch match = processor.CanProcess(acceptHeader, model, context);

                if (match.ModelResult != MatchResult.NoMatch && match.RequestedContentTypeResult != MatchResult.NoMatch)
                {
                    yield return new Tuple<IResponseProcessor, ProcessorMatch>(processor, match);
                }
            }
        }

        /// <summary>
        /// Creates a response from the compatible headers.
        /// </summary>
        /// <param name="compatibleHeaders">The compatible headers.</param>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Response"/>.</returns>
        private Response CreateResponse(IList<CompatibleHeader> compatibleHeaders,
                                        NegotiationContext negotiationContext,
                                        NancyContext context)
        {
            var response = NegotiateResponse(compatibleHeaders, negotiationContext, context);

            if (response == null)
            {
                context.WriteTraceLog(sb =>
                    sb.AppendLine("[DefaultResponseNegotiator] Unable to negotiate response - no processors returned valid response"));

                response = new NotAcceptableResponse();
            }

            response.WithHeader("Vary", "Accept");

            this.AddLinkHeader(compatibleHeaders, response, context.Request.Url);
            SetStatusCode(negotiationContext, response);
            SetReasonPhrase(negotiationContext, response);
            AddCookies(negotiationContext, response);

            if (response is NotAcceptableResponse)
            {
                return response;
            }

            AddContentTypeHeader(negotiationContext, response);
            AddNegotiatedHeaders(negotiationContext, response);

            return response;
        }

        /// <summary>
        /// Prioritizes the response processors and tries to negotiate a response.
        /// </summary>
        /// <param name="compatibleHeaders">The compatible headers.</param>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="context">The context.</param>
        /// <returns>Response.</returns>
        private static Response NegotiateResponse(
            IEnumerable<CompatibleHeader> compatibleHeaders,
            NegotiationContext negotiationContext,
            NancyContext context)
        {
            foreach (var compatibleHeader in compatibleHeaders)
            {
                var prioritizedProcessors = compatibleHeader.Processors
                    .OrderByDescending(x => x.Item2.ModelResult)
                    .ThenByDescending(x => x.Item2.RequestedContentTypeResult);

                foreach (var prioritizedProcessor in prioritizedProcessors)
                {
                    var processorType = prioritizedProcessor.Item1.GetType();

                    context.WriteTraceLog(sb =>
                        sb.AppendFormat("[DefaultResponseNegotiator] Invoking processor: {0}\n", processorType));

                    var mediaRangeModel = negotiationContext.GetModelForMediaRange(compatibleHeader.MediaRange);

                    var response = prioritizedProcessor.Item1.Process(compatibleHeader.MediaRange, mediaRangeModel, context);
                    if (response != null)
                    {
                        return response;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a link header to the <see cref="Response"/>.
        /// </summary>
        /// <param name="compatibleHeaders">The compatible headers.</param>
        /// <param name="response">The response.</param>
        /// <param name="requestUrl">The request URL.</param>
        private void AddLinkHeader(IEnumerable<CompatibleHeader> compatibleHeaders, Response response, Url requestUrl)
        {
            var linkProcessors = GetLinkProcessors(compatibleHeaders, response.ContentType);
            if (linkProcessors.Any())
            {
                string existingLinkHeader;
                response.Headers.TryGetValue("Link", out existingLinkHeader);
                response.Headers["Link"] = this.CreateLinkHeader(requestUrl, linkProcessors, existingLinkHeader);
            }
        }

        /// <summary>
        /// Gets the link processors based on the compatible headers and content-type.
        /// </summary>
        /// <param name="compatibleHeaders">The compatible headers.</param>
        /// <param name="contentType">The content-type of the response.</param>
        /// <returns>Dictionary{System.String, MediaRange}.</returns>
        private static IDictionary<string, MediaRange> GetLinkProcessors(
            IEnumerable<CompatibleHeader> compatibleHeaders,
            string contentType)
        {
            var linkProcessors = new Dictionary<string, MediaRange>();

            foreach (var header in compatibleHeaders)
            {
                foreach (var processor in header.Processors)
                {
                    foreach (var mapping in processor.Item1.ExtensionMappings)
                    {
                        if (!mapping.Item2.Matches(contentType))
                        {
                            linkProcessors[mapping.Item1] = mapping.Item2;
                        }
                    }
                }
            }

            return linkProcessors;
        }

        /// <summary>
        /// Creates the link header with the different media ranges.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="linkProcessors">The link processors.</param>
        /// <param name="existingLinkHeader">The existing Link HTTP Header.</param>
        /// <returns>The link header.</returns>
        protected virtual string CreateLinkHeader(Url requestUrl, IEnumerable<KeyValuePair<string, MediaRange>> linkProcessors, string existingLinkHeader)
        {
            var fileName = HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(requestUrl.Path));
            var baseUrl = string.Concat(requestUrl.BasePath, "/", fileName);
            var linkBuilder = new HttpLinkBuilder();

            if (existingLinkHeader != null)
            {
                linkBuilder.Add(existingLinkHeader);
            }

            foreach (var linkProcessor in linkProcessors)
            {
                var uri = string.Concat(baseUrl, '.', linkProcessor.Key);
                linkBuilder.Add(new HttpLink(uri, "alternate", linkProcessor.Value));
            }

            return linkBuilder.ToString();
        }

        /// <summary>
        /// Adds the content type header from the <see cref="NegotiationContext"/> to the <see cref="Response"/>.
        /// </summary>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="response">The response.</param>
        private static void AddContentTypeHeader(NegotiationContext negotiationContext, Response response)
        {
            string contentType;
            if (negotiationContext.Headers.TryGetValue("Content-Type", out contentType))
            {
                response.ContentType = contentType;
                negotiationContext.Headers.Remove("Content-Type");
            }
        }

        /// <summary>
        /// Adds the negotiated headers from the <see cref="NegotiationContext"/> to the <see cref="Response"/>.
        /// </summary>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="response">The response.</param>
        private static void AddNegotiatedHeaders(NegotiationContext negotiationContext, Response response)
        {
            foreach (var header in negotiationContext.Headers)
            {
                response.Headers[header.Key] = header.Value;
            }
        }

        /// <summary>
        /// Sets the status code from the <see cref="NegotiationContext"/> on the <see cref="Response"/>.
        /// </summary>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="response">The response.</param>
        private static void SetStatusCode(NegotiationContext negotiationContext, Response response)
        {
            if (negotiationContext.StatusCode.HasValue)
            {
                response.StatusCode = negotiationContext.StatusCode.Value;
            }
        }

        /// <summary>
        /// Sets the reason phrase from the <see cref="NegotiationContext"/> on the <see cref="Response"/>.
        /// </summary>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="response">The response.</param>
        private static void SetReasonPhrase(NegotiationContext negotiationContext, Response response)
        {
            if (negotiationContext.ReasonPhrase != null)
            {
                response.ReasonPhrase = negotiationContext.ReasonPhrase;
            }
        }

        /// <summary>
        /// Adds the cookies from the <see cref="NegotiationContext"/> to the <see cref="Response"/>.
        /// </summary>
        /// <param name="negotiationContext">The negotiation context.</param>
        /// <param name="response">The response.</param>
        private static void AddCookies(NegotiationContext negotiationContext, Response response)
        {
            foreach (var cookie in negotiationContext.Cookies)
            {
                response.Cookies.Add(cookie);
            }
        }

        private class CompatibleHeader
        {
            public CompatibleHeader(
                string mediaRange,
                IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> processors)
            {
                this.MediaRange = mediaRange;
                this.Processors = processors;
            }

            public string MediaRange { get; private set; }

            public IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> Processors { get; private set; }
        }
    }
}
