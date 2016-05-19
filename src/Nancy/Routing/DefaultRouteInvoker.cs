namespace Nancy.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.ErrorHandling;
    using Nancy.Extensions;
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
        /// <returns>A <see cref="Response"/> instance that represents the result of the invoked route.</returns>
        public async Task<Response> Invoke(Route route, CancellationToken cancellationToken, DynamicDictionary parameters, NancyContext context)
        {
            object result;

            try
            {
                result = await route.Invoke(parameters, cancellationToken).ConfigureAwait(false);
            }
            catch(RouteExecutionEarlyExitException earlyExitException)
            {
                context.WriteTraceLog(
                    sb => sb.AppendFormat(
                            "[DefaultRouteInvoker] Caught RouteExecutionEarlyExitException - reason {0}",
                            earlyExitException.Reason));
                return earlyExitException.Response;
            }

            if (!(result is ValueType) && result == null)
            {
                context.WriteTraceLog(
                    sb => sb.AppendLine("[DefaultRouteInvoker] Invocation of route returned null"));

                result = new Response();
            }

            return this.negotiator.NegotiateResponse(result, context);
        }
    }
}
