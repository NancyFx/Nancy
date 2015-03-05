namespace Nancy.Routing
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the functionality for invoking a <see cref="Route"/> and returning a <see cref="Response"/>
    /// </summary>
    public interface IRouteInvoker
    {
        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <param name="context">The context of the route that is being invoked.</param>
        /// <returns>A <see cref="Response"/> instance that represents the result of the invoked route.</returns>
        Task<Response> Invoke(Route route, CancellationToken cancellationToken, DynamicDictionary parameters, NancyContext context);
    }
}