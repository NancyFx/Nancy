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
<<<<<<< HEAD
        /// <param name="request">Incoming request</param>
        /// <param name="cache">Route cache</param>
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary</returns>
        Tuple<Route, DynamicDictionary> Resolve(Request request, IRouteCache cache);
=======
        /// <param name="context">Current context</param>
        /// <param name="routeCache">Route cache</param>
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary, Item3 being the prereq, Item4 being the postreq</returns>
        Tuple<Route, DynamicDictionary, Func<NancyContext, Response>, Action<NancyContext>> Resolve(NancyContext context, IRouteCache routeCache);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
    }
}