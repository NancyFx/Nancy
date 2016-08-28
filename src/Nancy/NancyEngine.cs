namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.Bootstrapper;
    using Cookies;
    using Diagnostics;
    using ErrorHandling;
    using Routing;

    using Helpers;
    using Nancy.Configuration;
    using Responses.Negotiation;

    /// <summary>
    /// Default engine for handling Nancy <see cref="Request"/>s.
    /// </summary>
    public class NancyEngine : INancyEngine
    {
        /// <summary>
        /// Key for error type
        /// </summary>
        public const string ERROR_KEY = "ERROR_TRACE";

        /// <summary>
        /// Key for error exception message
        /// </summary>
        public const string ERROR_EXCEPTION = "ERROR_EXCEPTION";

        private readonly IRequestDispatcher dispatcher;
        private readonly INancyContextFactory contextFactory;
        private readonly IRequestTracing requestTracing;
        private readonly IReadOnlyCollection<IStatusCodeHandler> statusCodeHandlers;
        private readonly IStaticContentProvider staticContentProvider;
        private readonly IResponseNegotiator negotiator;
        private readonly CancellationTokenSource engineDisposedCts;
        private readonly TraceConfiguration traceConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="dispatcher">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="statusCodeHandlers">Error handlers</param>
        /// <param name="requestTracing">The request tracing instance.</param>
        /// <param name="staticContentProvider">The provider to use for serving static content</param>
        /// <param name="negotiator">The response negotiator.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public NancyEngine(IRequestDispatcher dispatcher,
            INancyContextFactory contextFactory,
            IEnumerable<IStatusCodeHandler> statusCodeHandlers,
            IRequestTracing requestTracing,
            IStaticContentProvider staticContentProvider,
            IResponseNegotiator negotiator,
            INancyEnvironment environment)
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

            if (requestTracing == null)
            {
                throw new ArgumentNullException("requestTracing");
            }

            if (staticContentProvider == null)
            {
                throw new ArgumentNullException("staticContentProvider");
            }

            if (negotiator == null)
            {
                throw new ArgumentNullException("negotiator");
            }

            this.dispatcher = dispatcher;
            this.contextFactory = contextFactory;
            this.statusCodeHandlers = statusCodeHandlers.ToArray();
            this.requestTracing = requestTracing;
            this.staticContentProvider = staticContentProvider;
            this.negotiator = negotiator;
            this.engineDisposedCts = new CancellationTokenSource();
            this.traceConfiguration = environment.GetValue<TraceConfiguration>();
        }

        /// <summary>
        /// Factory for creating an <see cref="IPipelines"/> instance for a incoming request.
        /// </summary>
        /// <value>An <see cref="IPipelines"/> instance.</value>
        public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(this.engineDisposedCts.Token, cancellationToken))
            {
                cts.Token.ThrowIfCancellationRequested();

                if (request == null)
                {
                    throw new ArgumentNullException("request", "The request parameter cannot be null.");
                }

                var context = this.contextFactory.Create(request);

                if (preRequest != null)
                {
                    context = preRequest(context);
                }

                var staticContentResponse = this.staticContentProvider.GetContent(context);
                if (staticContentResponse != null)
                {
                    context.Response = staticContentResponse;

                    return context;
                }

                var pipelines = this.RequestPipelinesFactory.Invoke(context);

                var nancyContext = await this.InvokeRequestLifeCycle(context, cts.Token, pipelines)
                    .ConfigureAwait(false);

                this.CheckStatusCodeHandler(nancyContext);

                this.SaveTraceInformation(nancyContext);

                return nancyContext;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.engineDisposedCts.Cancel();
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

            ctx.Trace.RequestData = ctx.Request;
            ctx.Trace.ResponseData = ctx.Response;

            this.requestTracing.AddRequestDiagnosticToSession(sessionGuid, ctx);

            this.UpdateTraceCookie(ctx, sessionGuid);
        }

        private bool EnableTracing(NancyContext ctx)
        {
            return this.traceConfiguration.Enabled && !ctx.Items.ContainsKey(DiagnosticsHook.ItemsKey);
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
            var cookie = new NancyCookie("__NCTRACE", sessionGuid.ToString(), true)
            {
                Expires = DateTime.Now.AddMinutes(30)
            };

            ctx.Response = ctx.Response.WithCookie(cookie);
        }

        private void CheckStatusCodeHandler(NancyContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            IStatusCodeHandler defaultHandler = null;
            IStatusCodeHandler customHandler = null;

            foreach (var statusCodeHandler in this.statusCodeHandlers)
            {
                if (!statusCodeHandler.HandlesStatusCode(context.Response.StatusCode, context))
                {
                    continue;
                }

                if (defaultHandler == null && (statusCodeHandler is DefaultStatusCodeHandler))
                {
                    defaultHandler = statusCodeHandler;
                    continue;
                }

                if (customHandler == null && !(statusCodeHandler is DefaultStatusCodeHandler))
                {
                    customHandler = statusCodeHandler;
                    continue;
                }

                if ((defaultHandler != null) && (customHandler != null))
                {
                    break;
                }
            }

            var handler = customHandler ?? defaultHandler;

            if (handler == null)
            {
                return;
            }

            try
            {
                handler.Handle(context.Response.StatusCode, context);
            }
            catch (Exception ex)
            {
                if (defaultHandler == null)
                {
                    throw;
                }

                defaultHandler.Handle(context.Response.StatusCode, context);
            }
        }

        private async Task<NancyContext> InvokeRequestLifeCycle(NancyContext context, CancellationToken cancellationToken, IPipelines pipelines)
        {
            try
            {
                var response = await InvokePreRequestHook(context, cancellationToken, pipelines.BeforeRequest).ConfigureAwait(false) ??
                               await this.dispatcher.Dispatch(context, cancellationToken).ConfigureAwait(false);

                context.Response = response;

                await this.InvokePostRequestHook(context, cancellationToken, pipelines.AfterRequest).ConfigureAwait(false);

                await response.PreExecute(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.InvokeOnErrorHook(context, pipelines.OnError, ex);
            }

            return context;
        }

        private static Task<Response> InvokePreRequestHook(NancyContext context, CancellationToken cancellationToken, BeforePipeline pipeline)
        {
            return pipeline == null ? Task.FromResult<Response>(null) : pipeline.Invoke(context, cancellationToken);
        }

        private Task InvokePostRequestHook(NancyContext context, CancellationToken cancellationToken, AfterPipeline pipeline)
        {
            return pipeline == null ? TaskHelpers.CompletedTask : pipeline.Invoke(context, cancellationToken);
        }

        private void InvokeOnErrorHook(NancyContext context, ErrorPipeline pipeline, Exception ex)
        {
            try
            {
                if (pipeline == null)
                {
                    throw new RequestExecutionException(ex);
                }

                var onErrorResult = pipeline.Invoke(context, ex);

                if (onErrorResult == null)
                {
                    throw new RequestExecutionException(ex);
                }

                context.Response = this.negotiator.NegotiateResponse(onErrorResult, context);
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
