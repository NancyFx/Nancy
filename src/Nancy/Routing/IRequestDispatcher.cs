namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Responses.Negotiation;
    using ResolveResult = System.Tuple<Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    public interface IRequestDispatcher
    {
        void Dispatch(NancyContext context);
    }

    public class DefaultRequestDispatcher : IRequestDispatcher
    {
        private readonly IRouteResolver routeResolver;
        private readonly IEnumerable<IResponseProcessor> responseProcessors;
        private readonly IRouteInvoker routeInvoker;

        public DefaultRequestDispatcher(IRouteResolver routeResolver, IEnumerable<IResponseProcessor> responseProcessors, IRouteInvoker routeInvoker)
        {
            this.routeResolver = routeResolver;
            this.responseProcessors = responseProcessors;
            this.routeInvoker = routeInvoker;
        }

        public void Dispatch(NancyContext context)
        {
            var resolveResult = this.Resolve(context);

            context.Parameters = resolveResult.Item2;
            var resolveResultPreReq = resolveResult.Item3;
            var resolveResultPostReq = resolveResult.Item4;
            ExecuteRoutePreReq(context, resolveResultPreReq);

            if (context.Response == null)
            {
                context.Response = this.routeInvoker.Invoke(resolveResult.Item1, resolveResult.Item2, context);
            }

            if (context.Request.Method.ToUpperInvariant() == "HEAD")
            {
                context.Response = new HeadResponse(context.Response);
            }

            if (resolveResultPostReq != null)
            {
                resolveResultPostReq.Invoke(context);
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

        private Tuple<Route, DynamicDictionary, Func<NancyContext, Response>, Action<NancyContext>> Resolve(NancyContext context)
        {
            var extension =
                Path.GetExtension(context.Request.Path);

            var originalAcceptHeaders = context.Request.Headers.Accept;
            var originalRequestPath = context.Request.Path;

            if (!string.IsNullOrEmpty(extension))
            {
                var mappedMediaRanges =
                    this.GetMediaRangesForExtension(extension.Substring(1));

                if (mappedMediaRanges.Any())
                {
                    var newMediaRanges =
                        mappedMediaRanges.Where(x => !context.Request.Headers.Accept.Any(header => header.Equals(x)));

                    var modifiedAcceptHeaders =
                        context.Request.Headers.Accept.Concat(newMediaRanges);

                    var modifiedRequestPath = 
                        context.Request.Path.Replace(extension, string.Empty);

                    var match =
                        this.InvokeRouteResolver(context, modifiedRequestPath, modifiedAcceptHeaders);

                    if (!(match.Item1 is NotFoundRoute))
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
                .Select(mapping => new Tuple<string, decimal>(mapping.Item2, 1.1m));
        }

        private ResolveResult InvokeRouteResolver(NancyContext context, string path, IEnumerable<Tuple<string, decimal>> acceptHeaders)
        {
            context.Request.Headers.Accept = acceptHeaders.ToList();
            context.Request.Url.Path = path;

            return this.routeResolver.Resolve(context);
        }
    }
}