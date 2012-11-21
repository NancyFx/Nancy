namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Bootstrapper;

    using Nancy.Cookies;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Routing;

    /// <summary>
    /// Default engine for handling Nancy <see cref="Request"/>s.
    /// </summary>
    public class NancyEngine : INancyEngine
    {
        public const string ERROR_KEY = "ERROR_TRACE";
        public const string ERROR_EXCEPTION = "ERROR_EXCEPTION";

        private readonly IRequestDispatcher dispatcher;
        private readonly INancyContextFactory contextFactory;
        private readonly IRequestTracing requestTracing;
        private readonly IEnumerable<IStatusCodeHandler> statusCodeHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="dispatcher">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="statusCodeHandlers">Error handlers</param>
        /// <param name="requestTracing">The request tracing instance.</param>
        public NancyEngine(IRequestDispatcher dispatcher, INancyContextFactory contextFactory, IEnumerable<IStatusCodeHandler> statusCodeHandlers, IRequestTracing requestTracing)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher", "The resolver parameter cannot be null.");
            }

            if (contextFactory == null)
            {
                throw new ArgumentNullException("contextFactory");
            }

            if (statusCodeHandlers == null)
            {
                throw new ArgumentNullException("statusCodeHandlers");
            }

            this.dispatcher = dispatcher;
            this.contextFactory = contextFactory;
            this.statusCodeHandlers = statusCodeHandlers;
            this.requestTracing = requestTracing;
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
            return this.HandleRequest(request, context => context);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        private NancyContext HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var context = this.contextFactory.Create();
            context.Request = request;

            if (preRequest != null)
            {
                context = preRequest(context);
            }

            var pipelines =
                this.RequestPipelinesFactory.Invoke(context);

            this.InvokeRequestLifeCycle(context, pipelines);

            this.CheckStatusCodeHandler(context);

            this.SaveTraceInformation(context);

            return context;
        }

        private void SaveTraceInformation(NancyContext ctx)
        {
            if (!this.EnableTracing(ctx))
            {
                return;
            }

            if (ctx.Request == null || ctx.Response == null)
            {
                return;
            }

            var sessionGuid = this.GetDiagnosticsSessionGuid(ctx);

            ctx.Trace.ResponseType = ctx.Response.GetType();
            ctx.Trace.StatusCode = ctx.Response.StatusCode;
            ctx.Trace.RequestContentType = ctx.Request.Headers.ContentType;
            ctx.Trace.ResponseContentType = ctx.Response.ContentType;
            ctx.Trace.RequestHeaders = ctx.Request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
            ctx.Trace.ResponseHeaders = ctx.Response.Headers;

            this.requestTracing.AddRequestDiagnosticToSession(sessionGuid, ctx);

            this.UpdateTraceCookie(ctx, sessionGuid);
        }

        private bool EnableTracing(NancyContext ctx)
        {
            return StaticConfiguration.EnableRequestTracing &&
                   !ctx.Request.Path.StartsWith(DiagnosticsHook.ControlPanelPrefix);
        }

        private Guid GetDiagnosticsSessionGuid(NancyContext ctx)
        {
            string sessionId;
            if (!ctx.Request.Cookies.TryGetValue("__NCTRACE", out sessionId))
            {
                return this.requestTracing.CreateSession();
            }

            Guid sessionGuid;
            if (!Guid.TryParse(sessionId, out sessionGuid))
            {
                return this.requestTracing.CreateSession();
            }

            if (!this.requestTracing.IsValidSessionId(sessionGuid))
            {
                return this.requestTracing.CreateSession();
            }

            return sessionGuid;
        }

        private void UpdateTraceCookie(NancyContext ctx, Guid sessionGuid)
        {
            var cookie = new NancyCookie("__NCTRACE", sessionGuid.ToString(), true) { Expires = DateTime.Now.AddMinutes(30) };
            ctx.Response.AddCookie(cookie);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public void HandleRequest(Request request, Action<NancyContext> onComplete, Action<Exception> onError)
        {
            this.HandleRequest(request, context => context, onComplete, onError);
        }

        public void HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, Action<NancyContext> onComplete, Action<Exception> onError)
        {
            // TODO - potentially do some things sync like the pre-req hooks?
            // Possibly not worth it as the thread pool is quite clever
            // when it comes to fast running tasks such as ones where the prehook returns a redirect.
            ThreadPool.QueueUserWorkItem(s =>
            {
                try
                {
                    onComplete.Invoke(this.HandleRequest(request, preRequest));
                }
                catch (Exception e)
                {
                    onError.Invoke(e);
                }
            });
        }

        private void CheckStatusCodeHandler(NancyContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            foreach (var statusCodeHandler in this.statusCodeHandlers.Where(e => e.HandlesStatusCode(context.Response.StatusCode, context)))
            {
                statusCodeHandler.Handle(context.Response.StatusCode, context);
            }
        }

        private void InvokeRequestLifeCycle(NancyContext context, IPipelines pipelines)
        {
            try
            {
                InvokePreRequestHook(context, pipelines.BeforeRequest);

                if (context.Response == null) 
                {
                    this.dispatcher.Dispatch(context);
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
                    throw new RequestExecutionException(ex);
                }

                var onErrorResponse = pipeline.Invoke(context, ex);

                if (onErrorResponse == null)
                {
                    throw new RequestExecutionException(ex);
                }

                context.Response = onErrorResponse;
            }
            catch (Exception e)
            {
                context.Response = new Response { StatusCode = HttpStatusCode.InternalServerError };
                context.Items[ERROR_KEY] = e.ToString();
                context.Items[ERROR_EXCEPTION] = e;
            }
        }
    }
}
