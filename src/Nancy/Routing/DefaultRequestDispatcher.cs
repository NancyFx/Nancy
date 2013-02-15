namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Responses.Negotiation;

    /// <summary>
    /// Default implementation of a request dispatcher.
    /// </summary>
    public class DefaultRequestDispatcher : IRequestDispatcher
    {
        private readonly IRouteResolver routeResolver;
        private readonly IEnumerable<IResponseProcessor> responseProcessors;
        private readonly IRouteInvoker routeInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestDispatcher"/> class, with
        /// the provided <paramref name="routeResolver"/>, <paramref name="responseProcessors"/> and <paramref name="routeInvoker"/>.
        /// </summary>
        /// <param name="routeResolver"></param>
        /// <param name="responseProcessors"></param>
        /// <param name="routeInvoker"></param>
        public DefaultRequestDispatcher(IRouteResolver routeResolver, IEnumerable<IResponseProcessor> responseProcessors, IRouteInvoker routeInvoker)
        {
            this.routeResolver = routeResolver;
            this.responseProcessors = responseProcessors;
            this.routeInvoker = routeInvoker;
        }

        /// <summary>
        /// Dispatches a requests.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> for the current request.</param>
        public void Dispatch(NancyContext context)
        {
            var resolveResult = this.Resolve(context);

            context.Parameters = resolveResult.Parameters;

            try
            {
                ExecuteRoutePreReq(context, resolveResult.Before);

                if (context.Response == null)
                {
                    context.Response = this.routeInvoker.Invoke(resolveResult.Route, resolveResult.Parameters, context);
                }

                if (context.Request.Method.ToUpperInvariant() == "HEAD")
                {
                    context.Response = new HeadResponse(context.Response);
                }

                if (resolveResult.After != null)
                {
                    resolveResult.After.Invoke(context);
                }
            }
            catch (Exception exception)
            {
                var response = ResolveErrorResult(context, resolveResult.OnError, exception);

                if (response != null)
                {
                    context.Response = response;
                }
                else
                {
                    throw;
                }
            }
        }

        private static void ExecuteRoutePreReq(NancyContext context, Func<NancyContext, Response> resolveResultPreReq)
        {
            if (resolveResultPreReq == null)
            {
                return;
            }

            var resolveResultPreReqResponse = resolveResultPreReq.Invoke(context);

            if (resolveResultPreReqResponse != null)
            {
                context.Response = resolveResultPreReqResponse;
            }
        }

        private static Response ResolveErrorResult(NancyContext context, Func<NancyContext, Exception, Response> resolveResultOnError, Exception exception)
        {
            if (resolveResultOnError != null)
            {
                return resolveResultOnError.Invoke(context, exception);
            }

            return null;
        }

        private ResolveResult Resolve(NancyContext context)
        {
            var extension =
                Path.GetExtension(context.Request.Path);

            var originalAcceptHeaders = context.Request.Headers.Accept;
            var originalRequestPath = context.Request.Path;

            if (!string.IsNullOrEmpty(extension))
            {
                var mappedMediaRanges = this.GetMediaRangesForExtension(extension.Substring(1))
                    .ToArray();

                if (mappedMediaRanges.Any())
                {
                    var newMediaRanges =
                        mappedMediaRanges.Where(x => !context.Request.Headers.Accept.Any(header => header.Equals(x)));

                    var modifiedRequestPath = 
                        context.Request.Path.Replace(extension, string.Empty);

                    var match =
                        this.InvokeRouteResolver(context, modifiedRequestPath, newMediaRanges);

                    if (!(match.Route is NotFoundRoute))
                    {
                        return match;
                    }
                }
            }

            return this.InvokeRouteResolver(context, originalRequestPath, originalAcceptHeaders);
        }

        private IEnumerable<Tuple<string, decimal>> GetMediaRangesForExtension(string extension)
        {
            return this.responseProcessors
                .SelectMany(processor => processor.ExtensionMappings)
                .Where(mapping => mapping != null)
                .Where(mapping => mapping.Item1.Equals(extension, StringComparison.OrdinalIgnoreCase))
                .Select(mapping => new Tuple<string, decimal>(mapping.Item2, Decimal.MaxValue))
                .Distinct();
        }

        private ResolveResult InvokeRouteResolver(NancyContext context, string path, IEnumerable<Tuple<string, decimal>> acceptHeaders)
        {
            context.Request.Headers.Accept = acceptHeaders.ToList();
            context.Request.Url.Path = path;

            return this.routeResolver.Resolve(context);
        }
    }
}