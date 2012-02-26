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
        /// <param name="context">Current context</param>
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary, Item3 being the prereq, Item4 being the postreq</returns>
        Tuple<Route, DynamicDictionary, Func<NancyContext, Response>, Action<NancyContext>> Resolve(NancyContext context);
    }
}