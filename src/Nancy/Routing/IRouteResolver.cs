namespace Nancy.Routing
{
    /// <summary>
    /// Returns a route that matches the request
    /// </summary>
    public interface IRouteResolver
    {
        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>A <see cref="ResolveResult"/> containing the resolved route information.</returns>
        ResolveResult Resolve(NancyContext context);
    }
}
