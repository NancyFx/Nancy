namespace Nancy.Routing
{
    /// <summary>
    /// Defined the functionality that is required by a route pattern matcher.
    /// </summary>
    /// <remarks>Implement this interface if you want to support a custom route syntax.</remarks>
    public interface IRoutePatternMatcher
    {
        /// <summary>
        /// Attempts to match a requested path with a route pattern.
        /// </summary>
        /// <param name="requestedPath">The path that was requested.</param>
        /// <param name="routePath">The route pattern that the requested path should be attempted to be matched with.</param>
        /// <returns>An <see cref="IRoutePatternMatchResult"/> instance, containing the outcome of the match.</returns>
        IRoutePatternMatchResult Match(string requestedPath, string routePath);
    }
}