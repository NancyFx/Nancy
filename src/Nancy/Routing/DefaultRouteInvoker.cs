namespace Nancy.Routing
{
    /// <summary>
    /// Default route invoker implementation-
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <returns>A <see cref="Response"/> intance that represents the result of the invoked route.</returns>
        public Response Invoke(Route route, DynamicDictionary parameters)
        {
            var result =
                route.Invoke(parameters);

            return result;
        }
    }
}