namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Returns a route that matches the request
    /// </summary>
    public interface IRouteResolver
    {
        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="cache">Route cache</param>
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary</returns>
        Tuple<Route, DynamicDictionary> Resolve(Request request, IRouteCache cache);
    }
}