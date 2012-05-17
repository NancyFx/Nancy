namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Default route invoker implementation-
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        private readonly IEnumerable<ISerializer> serializers;

        public DefaultRouteInvoker(IEnumerable<ISerializer> serializers)
        {
            this.serializers = serializers;
        }

        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <param name="context">The context of the route that is being invoked.</param>
        /// <returns>A <see cref="Response"/> intance that represents the result of the invoked route.</returns>
        public Response Invoke(Route route, DynamicDictionary parameters, NancyContext context)
        {
            var result =
                route.Invoke(parameters);

            return result;
        }
    }
}