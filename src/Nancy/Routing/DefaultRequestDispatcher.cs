namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Responses.Negotiation;

    /// <summary>
    /// Default implementation of a request dispatcher.
    /// </summary>
    public class DefaultRequestDispatcher : IRequestDispatcher
    {
        private readonly IRouteResolver routeResolver;
        private readonly IEnumerable<IResponseProcessor> responseProcessors;
        private readonly IRouteInvoker routeInvoker;
        private readonly IResponseNegotiator negotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestDispatcher"/> class, with
        /// the provided <paramref name="routeResolver"/>, <paramref name="responseProcessors"/> and <paramref name="routeInvoker"/>.
        /// </summary>
        /// <param name="routeResolver"></param>
        /// <param name="responseProcessors"></param>
        /// <param name="routeInvoker"></param>
        /// <param name="negotiator"></param>
        public DefaultRequestDispatcher(IRouteResolver routeResolver,
            IEnumerable<IResponseProcessor> responseProcessors,
            IRouteInvoker routeInvoker,
            IResponseNegotiator negotiator)
        {
            this.routeResolver = routeResolver;
            this.responseProcessors = responseProcessors;
            this.routeInvoker = routeInvoker;
            this.negotiator = negotiator;
        }

        /// <summary>
        /// Dispatches a requests.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> for the current request.</param>
        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            // TODO - May need to make this run off context rather than response .. seems a bit icky currently
            var tcs = new TaskCompletionSource<Response>();

            var resolveResult = this.Resolve(context);

            context.Parameters = resolveResult.Parameters;
            context.ResolvedRoute = resolveResult.Route;

            var preReqTask = ExecuteRoutePreReq(context, cancellationToken, resolveResult.Before);

            preReqTask.WhenCompleted(
                completedTask =>
                    {
                        context.Response = completedTask.Result;

                        if (context.Response == null)
                        {
                            var routeTask = this.routeInvoker.Invoke(resolveResult.Route, cancellationToken, resolveResult.Parameters, context);

                            routeTask.WhenCompleted(
                                completedRouteTask =>
                                    {
                                        context.Response = completedRouteTask.Result;

                                        if (context.Request.Method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
                                        {
                                            context.Response = new HeadResponse(context.Response);
                                        }

                                        this.ExecutePost(context, cancellationToken, resolveResult.After, resolveResult.OnError, tcs);
                                    },
                                this.HandleFaultedTask(context, resolveResult.OnError, tcs));

                            return;
                        }

                        this.ExecutePost(context, cancellationToken, resolveResult.After, resolveResult.OnError, tcs);
                    },
                this.HandleFaultedTask(context, resolveResult.OnError, tcs));

            return tcs.Task;
        }

        private void ExecutePost(NancyContext context, CancellationToken cancellationToken, AfterPipeline postHook, Func<NancyContext, Exception, dynamic> onError, TaskCompletionSource<Response> tcs)
        {
            if (postHook == null)
            {
                tcs.SetResult(context.Response);
                return;
            }

            postHook.Invoke(context, cancellationToken).WhenCompleted(
                completedTask => tcs.SetResult(context.Response),
                completedTask => this.HandlePostHookFaultedTask(context, onError, completedTask, tcs));
        }

        private void HandlePostHookFaultedTask(NancyContext context, Func<NancyContext, Exception, dynamic> onError, Task completedTask, TaskCompletionSource<Response> tcs)
        {
            var response = this.ResolveErrorResult(context, onError, completedTask.Exception);

            if (response != null)
            {
                context.Response = response;

                tcs.SetResult(response);
            }
            else
            {
                tcs.SetException(completedTask.Exception);
            }
        }

        private Action<Task<Response>> HandleFaultedTask(NancyContext context, Func<NancyContext, Exception, dynamic> onError, TaskCompletionSource<Response> tcs)
        {
            return task => this.HandlePostHookFaultedTask(context, onError, task, tcs);
        }

        private static Task<Response> ExecuteRoutePreReq(NancyContext context, CancellationToken cancellationToken, BeforePipeline resolveResultPreReq)
        {
            if (resolveResultPreReq == null)
            {
                return TaskHelpers.GetCompletedTask<Response>(null);
            }

            return resolveResultPreReq.Invoke(context, cancellationToken);
        }

        private Response ResolveErrorResult(NancyContext context, Func<NancyContext, Exception, dynamic> resolveResultOnError, Exception exception)
        {
            if (resolveResultOnError != null)
            {
                var flattenedException = exception.FlattenInnerExceptions();

                var result = resolveResultOnError.Invoke(context, flattenedException);
                if (result != null)
                {
                    return this.negotiator.NegotiateResponse(result, context);
                }
            }

            return null;
        }

        private ResolveResult Resolve(NancyContext context)
        {
            var extension = context.Request.Path.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ? null
               : Path.GetExtension(context.Request.Path);

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

                    var index = context.Request.Path.LastIndexOf(extension, StringComparison.Ordinal);

                    var modifiedRequestPath =
                        context.Request.Path.Remove (index, extension.Length);

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
