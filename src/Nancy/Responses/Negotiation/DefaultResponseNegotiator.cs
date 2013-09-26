namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Nancy.Conventions;
    using Nancy.Extensions;

    public class DefaultResponseNegotiator : IResponseNegotiator
    {
        private readonly IEnumerable<IResponseProcessor> processors;
        private readonly AcceptHeaderCoercionConventions coercionConventions;

        public DefaultResponseNegotiator(
            IEnumerable<IResponseProcessor> processors,
            AcceptHeaderCoercionConventions coercionConventions)
        {
            this.processors = processors;
            this.coercionConventions = coercionConventions;
        }

        public Response NegotiateResponse(dynamic result, NancyContext context)
        {
            var response = CastResultToResponse(result);
            if (response != null)
            {
                context.WriteTraceLog(sb => sb.AppendLine("[DefaultResponseNegotiator] Processing as real response"));

                return response;
            }

            return this.ProcessAsNegotiator(result, context);
        }

        private static Response CastResultToResponse(dynamic result)
        {
            try
            {
                return (Response) result;
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>> GetCompatibleProcessorsByHeader(
            string acceptHeader, dynamic model, NancyContext context)
        {
            foreach (var processor in this.processors)
            {
                var result = (ProcessorMatch) processor.CanProcess(acceptHeader, model, context);

                if (result.ModelResult != MatchResult.NoMatch && result.RequestedContentTypeResult != MatchResult.NoMatch)
                {
                    yield return new Tuple<IResponseProcessor, ProcessorMatch>(processor, result);
                }
            }
        }

        private static Response NegotiateResponse(
            IEnumerable<Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>> compatibleHeaders,
            Negotiator negotiator,
            NancyContext context)
        {
            foreach (var compatibleHeader in compatibleHeaders)
            {
                var prioritizedProcessors = compatibleHeader.Item2
                    .OrderByDescending(x => x.Item2.ModelResult)
                    .ThenByDescending(x => x.Item2.RequestedContentTypeResult);

                foreach (var prioritizedProcessor in prioritizedProcessors)
                {
                    var processorType = prioritizedProcessor.Item1.GetType();

                    context.WriteTraceLog(sb => 
                        sb.AppendFormat("[DefaultResponseNegotiator] Invoking processor: {0}\n", processorType));

                    var model = negotiator.NegotiationContext.GetModelForMediaRange(compatibleHeader.Item1);
                    
                    var response = prioritizedProcessor.Item1.Process(compatibleHeader.Item1, model, context);
                    if (response != null)
                    {
                        return response;
                    }
                }
            }

            return null;
        }

        private Response ProcessAsNegotiator(object routeResult, NancyContext context)
        {
            context.WriteTraceLog(sb => sb.AppendLine("[DefaultResponseNegotiator] Processing as negotiation"));

            var negotiator = GetNegotiator(routeResult, context);

            var coercedAcceptHeaders = this.GetCoercedAcceptHeaders(context).ToArray();

            context.WriteTraceLog(sb =>
                {
                    var allowableFormats = negotiator.NegotiationContext.PermissableMediaRanges
                        .Select(mr => mr.ToString()).Aggregate((t1, t2) => t1 + ", " + t2);

                    var originalAccept = context.Request.Headers["accept"].Any()
                        ? string.Join(", ", context.Request.Headers["accept"])
                        : "None";

                    var coercedAccept = coercedAcceptHeaders.Any()
                        ? coercedAcceptHeaders.Select(h => h.Item1).Aggregate((t1, t2) => t1 + ", " + t2)
                        : "None";

                    sb.AppendFormat("[DefaultResponseNegotiator] Original accept header: {0}\n", originalAccept);
                    sb.AppendFormat("[DefaultResponseNegotiator] Coerced accept header: {0}\n", coercedAccept);
                    sb.AppendFormat("[DefaultResponseNegotiator] Acceptable media ranges: {0}\n", allowableFormats);
                });

            var compatibleHeaders = this.GetCompatibleHeaders(coercedAcceptHeaders, context, negotiator);

            if (!compatibleHeaders.Any())
            {
                context.WriteTraceLog(
                    sb => sb.AppendLine("[DefaultResponseNegotiator] Unable to negotiate response - no headers compatible"));

                return new NotAcceptableResponse();
            }

            var response = NegotiateResponse(compatibleHeaders, negotiator, context);

            if (response == null)
            {
                context.WriteTraceLog(sb =>
                    sb.AppendLine("[DefaultResponseNegotiator] Unable to negotiate response - no processors returned valid response"));

                response = new NotAcceptableResponse();
            }

            response.WithHeader("Vary", "Accept");

            AddLinkHeaders(context, compatibleHeaders, response);

            if (!(response is NotAcceptableResponse))
            {
                CheckForContentTypeHeader(negotiator, response);
                AddNegotiatedHeaders(negotiator, response);
            }

            if (negotiator.NegotiationContext.StatusCode.HasValue)
            {
                response.StatusCode = negotiator.NegotiationContext.StatusCode.Value;
            }

            foreach (var cookie in negotiator.NegotiationContext.Cookies)
            {
                response.Cookies.Add(cookie);
            }

            return response;
        }

        private static void CheckForContentTypeHeader(Negotiator negotiator, Response response)
        {
            if (!negotiator.NegotiationContext.Headers.ContainsKey("Content-Type"))
            {
                return;
            }
            
            response.ContentType = negotiator.NegotiationContext.Headers["Content-Type"];
            negotiator.NegotiationContext.Headers.Remove("Content-Type");
        }

        private static void AddNegotiatedHeaders(Negotiator negotiator, Response response)
        {
            foreach (var header in negotiator.NegotiationContext.Headers)
            {
                response.Headers[header.Key] = header.Value;
            }
        }

        private static void AddLinkHeaders(
            NancyContext context,
            IEnumerable<Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>> compatibleHeaders,
            Response response)
        {
            var linkProcessors = new Dictionary<string, MediaRange>();

            var compatibleHeaderMappings = compatibleHeaders
                .SelectMany(m => m.Item2)
                .SelectMany(p => p.Item1.ExtensionMappings)
                .Where(m => !m.Item2.Matches(response.ContentType));

            foreach (var compatibleHeaderMapping in compatibleHeaderMappings)
            {
                linkProcessors[compatibleHeaderMapping.Item1] = compatibleHeaderMapping.Item2;
            }

            if (!linkProcessors.Any())
            {
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(context.Request.Url.Path);
            var baseUrl = string.Concat(context.Request.Url.BasePath, "/", fileName);

            var links = linkProcessors.Keys
                .Select(lp => CreateLink(baseUrl, lp, linkProcessors))
                .Aggregate((lp1, lp2) => lp1 + "," + lp2);

            response.Headers["Link"] = links;
        }

        private static string CreateLink(string baseUrl, string lp, IDictionary<string, MediaRange> linkProcessors)
        {
            return string.Format("<{0}.{1}>; rel=\"{2}\"", baseUrl, lp, linkProcessors[lp]);
        }

        private Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>[] GetCompatibleHeaders(
            IEnumerable<Tuple<string, decimal>> coercedAcceptHeaders,
            NancyContext context,
            Negotiator negotiator)
        {
            List<Tuple<string, decimal>> acceptHeaders;

            var permissableMediaRanges = negotiator.NegotiationContext.PermissableMediaRanges;

            if (permissableMediaRanges.Any(mr => mr.IsWildcard))
            {
                acceptHeaders = coercedAcceptHeaders.Where(header => header.Item2 > 0m).ToList();
            }
            else
            {
                acceptHeaders = coercedAcceptHeaders.Where(header => header.Item2 > 0m)
                    .SelectMany(header => permissableMediaRanges
                        .Where(mr => mr.Matches(header.Item1))
                        .Select(mr => Tuple.Create(mr.ToString(), header.Item2)))
                        .ToList();
            }

            return this.GetCompatibleProcessors(acceptHeaders, negotiator, context).ToArray();
        }

        private IEnumerable<Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>>
            GetCompatibleProcessors(
            IEnumerable<Tuple<string, decimal>> acceptHeaders,
            Negotiator negotiator,
            NancyContext context)
        {
            foreach (var header in acceptHeaders)
            {
                var compatibleProcessors =
                    (IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>)
                    this.GetCompatibleProcessorsByHeader(
                        header.Item1, negotiator.NegotiationContext.GetModelForMediaRange(header.Item1), context);

                if (compatibleProcessors.Any())
                {
                    yield return
                        new Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>(
                            header.Item1, compatibleProcessors);
                }
            }
        }

        private IEnumerable<Tuple<string, decimal>> GetCoercedAcceptHeaders(NancyContext context)
        {
            return this.coercionConventions.Aggregate(context.Request.Headers.Accept, (current, coercion) => coercion.Invoke(current, context));
        }

        private static Negotiator GetNegotiator(object routeResult, NancyContext context)
        {
            var negotiator = routeResult as Negotiator;
            if (negotiator != null)
            {
                return negotiator;
            }

            context.WriteTraceLog(sb => 
                sb.AppendFormat("[DefaultResponseNegotiator] Wrapping result of type {0} in negotiator\n", routeResult.GetType()));

            return new Negotiator(context).WithModel(routeResult);
        }
    }
}