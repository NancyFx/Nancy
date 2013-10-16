namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bootstrapper;

    using Nancy.Cookies;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Routing;

    using Nancy.Helpers;

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
        private readonly DiagnosticsConfiguration diagnosticsConfiguration;
        private readonly IEnumerable<IStatusCodeHandler> statusCodeHandlers;
        private readonly IStaticContentProvider staticContentProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="dispatcher">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="statusCodeHandlers">Error handlers</param>
        /// <param name="requestTracing">The request tracing instance.</param>
        /// <param name="diagnosticsConfiguration"></param>
        /// <param name="staticContentProvider">The provider to use for serving static content</param>
        public NancyEngine(IRequestDispatcher dispatcher, INancyContextFactory contextFactory, IEnumerable<IStatusCodeHandler> statusCodeHandlers, IRequestTracing requestTracing, DiagnosticsConfiguration diagnosticsConfiguration, IStaticContentProvider staticContentProvider)
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
            this.diagnosticsConfiguration = diagnosticsConfiguration;
            this.staticContentProvider = staticContentProvider;
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
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public void HandleRequest(Request request, Action<NancyContext> onComplete, Action<Exception> onError)
        {
            this.HandleRequest(request, null, onComplete, onError);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        private NancyContext HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest)
        {
            var task = this.HandleRequestInternal(request, preRequest);

            task.Wait();

            if (task.IsFaulted)
            {
                throw task.Exception ?? new Exception("Request task faulted");
            }

            return task.Result;
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Pre request hook from the host</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public void HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, Action<NancyContext> onComplete, Action<Exception> onError)
        {
            this.HandleRequestInternal(request, preRequest)
                .WhenCompleted(t => onComplete(t.Result), t => onError(t.Exception));
        }

        private Task<NancyContext> HandleRequestInternal(Request request, Func<NancyContext, NancyContext> preRequest)
        {
            // TODO - replace continuations with a fast continue from the pipeline spike

            var tcs = new TaskCompletionSource<NancyContext>();

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
                tcs.SetResult(context);
                return tcs.Task;
            }

            var pipelines =
                this.RequestPipelinesFactory.Invoke(context);

            // TODO - potentially get this passed in so requests can be cancelled
            var cancellationToken = new CancellationToken();
            context.Items["CANCELLATION_TOKEN"] = cancellationToken; // So we get disposed when the request is complete

            var task = this.InvokeRequestLifeCycle(context, cancellationToken, pipelines);

            task.WhenCompleted(
                completeTask =>
                {
                    this.CheckStatusCodeHandler(completeTask.Result);

                    this.SaveTraceInformation(completeTask.Result);

                    tcs.SetResult(completeTask.Result);
                },
                errorTask =>
                {
                    tcs.SetException(errorTask.Exception);
                },
                true);

            return tcs.Task;
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
                   !ctx.Items.ContainsKey(DiagnosticsHook.ItemsKey);
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

        private void CheckStatusCodeHandler(NancyContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            foreach (var statusCodeHandler in this.statusCodeHandlers)
            {
                if (statusCodeHandler.HandlesStatusCode(context.Response.StatusCode, context))
                {
                    statusCodeHandler.Handle(context.Response.StatusCode, context);
                }
            }
        }

        private Task<NancyContext> InvokeRequestLifeCycle(NancyContext context, CancellationToken cancellationToken, IPipelines pipelines)
        {
            var tcs = new TaskCompletionSource<NancyContext>();

            var preHookTask = InvokePreRequestHook(context, cancellationToken, pipelines.BeforeRequest);

            preHookTask.WhenCompleted(t =>
                {
                    var dispatchTask = t.Result != null ? TaskHelpers.GetCompletedTask(t.Result) : this.dispatcher.Dispatch(context, cancellationToken);

                    dispatchTask.WhenCompleted(
                        completedTask =>
                        {
                            context.Response = completedTask.Result;

                            var postHookTask = InvokePostRequestHook(context, cancellationToken, pipelines.AfterRequest);

                            postHookTask.WhenCompleted(
                                completedPostHookTask => tcs.SetResult(context),
                                HandleFaultedTask(context, pipelines, tcs));
                        },
                        HandleFaultedTask(context, pipelines, tcs));
                },
                HandleFaultedTask(context, pipelines, tcs));

            return tcs.Task;
        }

        private static Action<Task> HandleFaultedTask(NancyContext context, IPipelines pipelines, TaskCompletionSource<NancyContext> tcs)
        {
            return t =>
                {
                    try
                    {
                        var flattenedException = FlattenException(t.Exception);

                        InvokeOnErrorHook(context, pipelines.OnError, flattenedException);

                        tcs.SetResult(context);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                };
        }

        private static Task<Response> InvokePreRequestHook(NancyContext context, CancellationToken cancellationToken, BeforePipeline pipeline)
        {
            if (pipeline == null)
            {
                return TaskHelpers.GetCompletedTask<Response>(null);
            }

            return pipeline.Invoke(context, cancellationToken);
        }

        private Task InvokePostRequestHook(NancyContext context, CancellationToken cancellationToken, AfterPipeline pipeline)
        {
            return pipeline == null ? TaskHelpers.GetCompletedTask() : pipeline.Invoke(context, cancellationToken);
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

        private static Exception FlattenException(Exception exception)
        {
            if (exception is AggregateException)
            {
                var aggregateException = exception as AggregateException;

                var flattenedAggregateException = aggregateException.Flatten();

                //If we have more than one exception in the AggregateException
                //we have to send all exceptions back in order not to swallow any exceptions.
                if (flattenedAggregateException.InnerExceptions.Count > 1)
                {
                    return flattenedAggregateException;
                }

                return flattenedAggregateException.InnerException;
            }

            if (exception != null && exception.InnerException != null)
            {
                return FlattenException(exception.InnerException);
            }

            return exception;
        }
    }
}
