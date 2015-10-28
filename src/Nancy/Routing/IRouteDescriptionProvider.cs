namespace Nancy.Routing
{
    /// <summary>
    /// Defines the functionality for retrieving a description for a specific route.
    /// </summary>
    public interface IRouteDescriptionProvider
    {
        /// <summary>
        /// Get the description for a route.
        /// </summary>
        /// <param name="module">The module that the route is defined in.</param>
        /// <param name="path">The path of the route that the description should be retrieved for.</param>
        /// <returns>A <see cref="string"/> containing the description of the route if it could be found, otherwise <see cref="string.Empty"/>.</returns>
        string GetDescription(INancyModule module, string path);
    }
}