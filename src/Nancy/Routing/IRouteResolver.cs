namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Resolves routes for a given request
    /// </summary>
    public interface IRouteResolver
    {
        /// <summary>
        /// Gets a route for the given request
        /// </summary>
        /// <param name="request">Current request</param>
        /// <returns>Resolved route</returns>
        IRoute GetRoute(IRequest request);
    }
}