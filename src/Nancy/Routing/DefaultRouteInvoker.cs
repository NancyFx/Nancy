namespace Nancy.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.ErrorHandling;
    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Default route invoker implementation.
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        private readonly IResponseNegotiator negotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteInvoker"/> class.
        /// </summary>
        /// <param name="negotiator">The response negotiator.</param>
        public DefaultRouteInvoker(IResponseNegotiator negotiator)
        {
            this.negotiator = negotiator;
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
            if (isResponse)
            {
                return ProcessAsRealResponse(result, context);
            }

            context.WriteTraceLog(sb =>
                sb.AppendLine("[DefaultRouteInvoker] Processing as negotiation"));

            return this.negotiator.NegotiateResponse(result, context);
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

        private static Response ProcessAsRealResponse(dynamic routeResult, NancyContext context)
        {
            context.WriteTraceLog(sb => sb.AppendLine("[DefaultRouteInvoker] Processing as real response"));

            return (Response)routeResult;
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
