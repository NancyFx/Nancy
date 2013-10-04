namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.Conventions;
    using Nancy.ErrorHandling;
    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Default route invoker implementation.
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        private readonly IEnumerable<IResponseProcessor> processors;

        private readonly AcceptHeaderCoercionConventions coercionConventions;

        public DefaultRouteInvoker(
            IEnumerable<IResponseProcessor> processors, AcceptHeaderCoercionConventions coercionConventions)
        {
            this.processors = processors;
            this.coercionConventions = coercionConventions;
        }

        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <param name="context">The context of the route that is being invoked.</param>
        /// <returns>A <see cref="Response"/> intance that represents the result of the invoked route.</returns>
        public Task<Response> Invoke(Route route, CancellationToken cancellationToken, DynamicDictionary parameters, NancyContext context)
        {
            var tcs = new TaskCompletionSource<Response>();

            var result = route.Invoke(parameters, cancellationToken);

            result.WhenCompleted(
                completedTask =>
                {
                    var returnResult = completedTask.Result;
                    if (returnResult == null)
                    {
                        context.WriteTraceLog(
                            sb => sb.AppendLine("[DefaultRouteInvoker] Invocation of route returned null"));

                        returnResult = new Response();
                    }

                    try
                    {
                        var negotiatedResult = this.InvokeRouteWithStrategy(returnResult, context);

                        tcs.SetResult(negotiatedResult);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                },
                faultedTask =>
                {
                    var earlyExitException = GetEarlyExitException(faultedTask);

                    if (earlyExitException != null)
                    {
                        context.WriteTraceLog(
                            sb =>
                            sb.AppendFormat(
                                "[DefaultRouteInvoker] Caught RouteExecutionEarlyExitException - reason {0}",
                                earlyExitException.Reason));
                        tcs.SetResult(earlyExitException.Response);
                    }
                    else
                    {
                        tcs.SetException(faultedTask.Exception);
                    }
                });

            return tcs.Task;
        }

        private Response InvokeRouteWithStrategy(dynamic result, NancyContext context)
        {
            var isResponse = (CastResultToResponse(result) != null);

            return (isResponse) ? ProcessAsRealResponse(result, context) : this.ProcessAsNegotiator(result, context);
        }

        private static Response CastResultToResponse(dynamic result)
        {
            try
            {
                return (Response)result;
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
                var result = (ProcessorMatch)processor.CanProcess(acceptHeader, model, context);

                if (result.ModelResult != MatchResult.NoMatch
                    && result.RequestedContentTypeResult != MatchResult.NoMatch)
                {
                    yield return new Tuple<IResponseProcessor, ProcessorMatch>(processor, result);
                }
            }
        }

        private static Response ProcessAsRealResponse(dynamic routeResult, NancyContext context)
        {
            context.WriteTraceLog(sb => sb.AppendLine("[DefaultRouteInvoker] Processing as real response"));

            return (Response)routeResult;
        }

        private static Response NegotiateResponse(
            IEnumerable<Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>> compatibleHeaders,
            object model,
            Negotiator negotiator,
            NancyContext context)
        {
            foreach (var compatibleHeader in compatibleHeaders)
            {
                var prioritizedProcessors =
                    compatibleHeader.Item2.OrderByDescending(x => x.Item2.ModelResult)
                                    .ThenByDescending(x => x.Item2.RequestedContentTypeResult);

                foreach (var prioritizedProcessor in prioritizedProcessors)
                {
                    var processorType = prioritizedProcessor.Item1.GetType();
                    context.WriteTraceLog(
                        sb => sb.AppendFormat("[DefaultRouteInvoker] Invoking processor: {0}\n", processorType));

                    var response = prioritizedProcessor.Item1.Process(
                        compatibleHeader.Item1,
                        negotiator.NegotiationContext.GetModelForMediaRange(compatibleHeader.Item1),
                        context);

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
            context.WriteTraceLog(sb => sb.AppendLine("[DefaultRouteInvoker] Processing as negotiation"));

            var negotiator = GetNegotiator(routeResult, context);

            var coercedAcceptHeaders = this.GetCoercedAcceptHeaders(context).ToArray();

            context.WriteTraceLog(
                sb =>
                {
                    var allowableFormats =
                        negotiator.NegotiationContext.PermissableMediaRanges.Select(mr => mr.ToString())
                                  .Aggregate((t1, t2) => t1 + ", " + t2);

                    var originalAccept = context.Request.Headers["accept"].Any()
                                             ? string.Join(", ", context.Request.Headers["accept"])
                                             : "None";

                    var coercedAccept = coercedAcceptHeaders.Any()
                                            ? coercedAcceptHeaders.Select(h => h.Item1)
                                                                  .Aggregate((t1, t2) => t1 + ", " + t2)
                                            : "None";

                    sb.AppendFormat("[DefaultRouteInvoker] Original accept header: {0}\n", originalAccept);
                    sb.AppendFormat("[DefaultRouteInvoker] Coerced accept header: {0}\n", coercedAccept);
                    sb.AppendFormat("[DefaultRouteInvoker] Acceptable media ranges: {0}\n", allowableFormats);
                });

            var compatibleHeaders = this.GetCompatibleHeaders(coercedAcceptHeaders, context, negotiator);

            if (!compatibleHeaders.Any())
            {
                context.WriteTraceLog(
                    sb => sb.AppendLine("[DefaultRouteInvoker] Unable to negotiate response - no headers compatible"));

                return new NotAcceptableResponse();
            }

            var response = NegotiateResponse(compatibleHeaders, routeResult, negotiator, context);

            if (response == null)
            {
                context.WriteTraceLog(
                    sb =>
                    sb.AppendLine(
                        "[DefaultRouteInvoker] Unable to negotiate response - no processors returned valid response"));

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

            if (negotiator.NegotiationContext.ReasonPhrase != null)
            {
                response.ReasonPhrase = negotiator.NegotiationContext.ReasonPhrase;
            }

            foreach (var cookie in negotiator.NegotiationContext.Cookies)
            {
                response.Cookies.Add(cookie);
            }

            return response;
        }

        private static void CheckForContentTypeHeader(Negotiator negotiator, Response response)
        {
            if (negotiator.NegotiationContext.Headers.ContainsKey("Content-Type"))
            {
                response.ContentType = negotiator.NegotiationContext.Headers["Content-Type"];
                negotiator.NegotiationContext.Headers.Remove("Content-Type");
            }
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

            var compatibleHeaderMappings =
                compatibleHeaders.SelectMany(m => m.Item2).SelectMany(p => p.Item1.ExtensionMappings);

            foreach (var compatibleHeaderMapping in compatibleHeaderMappings)
            {
                if (!compatibleHeaderMapping.Item2.Matches(response.ContentType))
                {
                    linkProcessors[compatibleHeaderMapping.Item1] = compatibleHeaderMapping.Item2;
                }
            }

            if (!linkProcessors.Any())
            {
                return;
            }

            var baseUrl = context.Request.Url.BasePath + "/"
                          + Path.GetFileNameWithoutExtension(context.Request.Url.Path);

            var links =
                linkProcessors.Keys.Select(
                    lp => string.Format("<{0}.{1}>; rel=\"{2}\"", baseUrl, lp, linkProcessors[lp]))
                              .Aggregate((lp1, lp2) => lp1 + "," + lp2);

            response.Headers["Link"] = links;
        }

        private Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>[] GetCompatibleHeaders(
            IEnumerable<Tuple<string, decimal>> coercedAcceptHeaders, NancyContext context, Negotiator negotiator)
        {
            List<Tuple<string, decimal>> acceptHeaders;

            var permissableMediaRanges = negotiator.NegotiationContext.PermissableMediaRanges;

            if (permissableMediaRanges.Any(mr => mr.IsWildcard))
            {
                acceptHeaders = coercedAcceptHeaders.Where(header => header.Item2 > 0m).ToList();
            }
            else
            {
                acceptHeaders =
                    coercedAcceptHeaders.Where(header => header.Item2 > 0m)
                                        .SelectMany(
                                            header =>
                                            permissableMediaRanges.Where(mr => mr.Matches(header.Item1))
                                                                  .Select(
                                                                      mr => Tuple.Create(mr.ToString(), header.Item2)))
                                        .ToList();
            }

            return this.GetCompatibleProcessors(acceptHeaders, negotiator, context).ToArray();
        }

        private IEnumerable<Tuple<string, IEnumerable<Tuple<IResponseProcessor, ProcessorMatch>>>>
            GetCompatibleProcessors(
            IEnumerable<Tuple<string, decimal>> acceptHeaders, Negotiator negotiator, NancyContext context)
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
            var currentHeaders = context.Request.Headers.Accept;

            foreach (var coercion in coercionConventions)
            {
                currentHeaders = coercion.Invoke(currentHeaders, context);
            }

            return currentHeaders;
        }

        private static Negotiator GetNegotiator(object routeResult, NancyContext context)
        {
            var negotiator = routeResult as Negotiator;

            if (negotiator == null)
            {
                context.WriteTraceLog(
                    sb =>
                    sb.AppendFormat(
                        "[DefaultRouteInvoker] Wrapping result of type {0} in negotiator\n", routeResult.GetType()));

                negotiator = new Negotiator(context);
                negotiator.WithModel(routeResult);
            }

            return negotiator;
        }

        private static RouteExecutionEarlyExitException GetEarlyExitException(Task<dynamic> faultedTask)
        {
            var taskExceptions = faultedTask.Exception;

            if (taskExceptions == null)
            {
                return null;
            }

            if (taskExceptions.InnerExceptions.Count > 1)
            {
                return null;
            }

            return taskExceptions.InnerException as RouteExecutionEarlyExitException;
        }
    }
}
