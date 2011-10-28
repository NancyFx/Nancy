namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Bootstrapper;
    using Nancy.ErrorHandling;
    using Nancy.Routing;

    /// <summary>
    /// Default engine for handling Nancy <see cref="Request"/>s.
    /// </summary>
    public class NancyEngine : INancyEngine
    {
        internal const string ERROR_KEY = "ERROR_TRACE";

        private readonly IRouteResolver resolver;
        private readonly IRouteCache routeCache;
        private readonly INancyContextFactory contextFactory;
        private readonly IEnumerable<IErrorHandler> errorHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="routeCache">Cache of all available routes</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="errorHandlers">Error handlers</param>
        public NancyEngine(IRouteResolver resolver, IRouteCache routeCache, INancyContextFactory contextFactory, IEnumerable<IErrorHandler> errorHandlers)
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

            if (errorHandlers == null)
            {
                throw new ArgumentNullException("errorHandlers");
            }

            this.resolver = resolver;
            this.routeCache = routeCache;
            this.contextFactory = contextFactory;
            this.errorHandlers = errorHandlers;
        }

        /// <summary>
        /// Factory for creating an <see cref="IPipelines"/> instance for a incoming request.
        /// </summary>
        /// <value>An <see cref="IPipelines"/> instance.</value>
        public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

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

            var pipelines =
                this.RequestPipelinesFactory.Invoke(context);

            this.InvokeRequestLifeCycle(context, pipelines);
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
            ThreadPool.QueueUserWorkItem(s =>
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

            foreach (var errorHandler in this.errorHandlers.Where(e => e.HandlesStatusCode(context.Response.StatusCode)))
            {
                errorHandler.Handle(context.Response.StatusCode, context);
            }
        }

        private void InvokeRequestLifeCycle(NancyContext context, IPipelines pipelines)
        {
            try
            {
                InvokePreRequestHook(context, pipelines.BeforeRequest);

                if (context.Response == null) 
                {
                    this.ResolveAndInvokeRoute(context);
                }

                if (pipelines.AfterRequest != null) 
                {
                    pipelines.AfterRequest.Invoke(context);
                }
            }
            catch (Exception ex)
            {
                InvokeOnErrorHook(context, pipelines.OnError, ex);
            }
        }

        private static void InvokePreRequestHook(NancyContext context, BeforePipeline pipeline)
        {
            if (pipeline != null)
            {
                var preRequestResponse = pipeline.Invoke(context);

                if (preRequestResponse != null)
                {
                    context.Response = preRequestResponse;
                }
            }
        }

        private static void InvokeOnErrorHook(NancyContext context, ErrorPipeline pipeline, Exception ex)
        {
            try
            {
                if (pipeline == null)
                { 
                    throw ex;
                }

                var onErrorResponse = pipeline.Invoke(context, ex);

                if (onErrorResponse == null)
                {
                    throw ex;
                }

                context.Response = onErrorResponse;
            }
            catch (Exception e)
            {
                context.Response = new Response { StatusCode = HttpStatusCode.InternalServerError };
                context.Items[ERROR_KEY] = e.ToString();
            }
        }

        private void ResolveAndInvokeRoute(NancyContext context)
        {
            var resolveResult = this.resolver.Resolve(context, this.routeCache);

            context.Parameters = resolveResult.Item2; 
            var resolveResultPreReq = resolveResult.Item3;
            var resolveResultPostReq = resolveResult.Item4;
            ExecuteRoutePreReq(context, resolveResultPreReq);

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
    }
}
