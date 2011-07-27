namespace Nancy
{
    using System;
    using System.Threading;

    using Nancy.ErrorHandling;
    using Nancy.Routing;

    public class NancyEngine : INancyEngine
    {
        private readonly IRouteResolver resolver;
        private readonly IRouteCache routeCache;
        private readonly INancyContextFactory contextFactory;
        private readonly IErrorHandler errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="routeCache">Cache of all available routes</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="errorHandler">Error handler</param>
        public NancyEngine(IRouteResolver resolver, IRouteCache routeCache, INancyContextFactory contextFactory, IErrorHandler errorHandler)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            if (routeCache == null)
            {
                throw new ArgumentNullException("routeCache", "The routeCache parameter cannot be null.");
            }

            if (contextFactory == null)
            {
                throw new ArgumentNullException("contextFactory");
            }

            if (errorHandler == null)
            {
                throw new ArgumentNullException("errorHandler");
            }

            this.resolver = resolver;
            this.routeCache = routeCache;
            this.contextFactory = contextFactory;
            this.errorHandler = errorHandler;
        }

        /// <summary>
        /// <para>
        /// Gets or sets the pre-request hook.
        /// </para>
        /// <para>
        /// The Pre-request hook is called prior to processing a request. If a hook returns
        /// a non-null response then processing is aborted and the response provided is
        /// returned.
        /// </para>
        /// </summary>
        public Func<NancyContext, Response> PreRequestHook { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the post-requets hook.
        /// </para>
        /// <para>
        /// The post-request hook is called after a route is located and invoked. The post
        /// request hook can rewrite the response or add/remove items from the context
        /// </para>
        /// </summary>
        public Action<NancyContext> PostRequestHook { get; set; }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        public NancyContext HandleRequest(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var context = this.contextFactory.Create();
            context.Request = request;

            this.InvokeRequestLifeCycle(context);

            AddNancyVersionHeaderToResponse(context);

            CheckErrorHandler(context);

            return context;
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public void HandleRequest(Request request, Action<NancyContext> onComplete, Action<Exception> onError)
        {
            // TODO - potentially do some things sync like the pre-req hooks?
            // Possibly not worth it as the thread pool is quite clever
            // when it comes to fast running tasks such as ones where the prehook returns a redirect.
            ThreadPool.QueueUserWorkItem((s) =>
                {
                    try
                    {
                        onComplete.Invoke(this.HandleRequest(request));
                    }
                    catch (Exception e)
                    {
                        onError.Invoke(e);
                    }
                });
        }

        private static void AddNancyVersionHeaderToResponse(NancyContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            var version =
                typeof(INancyEngine).Assembly.GetName().Version;

            context.Response.Headers["Nancy-Version"] = version.ToString();
        }

        private void CheckErrorHandler(NancyContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            if (this.errorHandler.HandlesStatusCode(context.Response.StatusCode))
            {
                this.errorHandler.Handle(context.Response.StatusCode, context);
            }
        }

        private void InvokeRequestLifeCycle(NancyContext context)
        {
            this.InvokePreRequestHook(context);

            if (context.Response == null)
            {
                this.ResolveAndInvokeRoute(context);
            }

            if (this.PostRequestHook != null)
            {
                this.PostRequestHook.Invoke(context);
            }
        }

        private void InvokePreRequestHook(NancyContext context)
        {
            if (this.PreRequestHook != null)
            {
                var preRequestResponse = this.PreRequestHook.Invoke(context);

                if (preRequestResponse != null)
                {
                    context.Response = preRequestResponse;
                }
            }
        }

        private void ResolveAndInvokeRoute(NancyContext context)
        {
            var resolveResult = this.resolver.Resolve(context, this.routeCache);
            var resolveResultPreReq = resolveResult.Item3;
            var resolveResultPostReq = resolveResult.Item4;

            this.ExecuteRoutePreReq(context, resolveResultPreReq);

            if (context.Response == null)
            {
                context.Response = resolveResult.Item1.Invoke(resolveResult.Item2);
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

        private void ExecuteRoutePreReq(NancyContext context, Func<NancyContext, Response> resolveResultPreReq)
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
    }
}