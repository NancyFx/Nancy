namespace Nancy.Routing
{
    /// <summary>
    /// Defines the functionality that is requred by a route pattern match result.
    /// </summary>
    public interface IRoutePatternMatchResult
    {
        /// <summary>
        /// Gets a value idicating wether or not a match was made.
        /// </summary>
        /// <value><see langword="true"/> if a match was made; otherwise <see langword="false"/>.</value>
        bool IsMatch { get; }

        /// <summary>
        /// The parameters that could be captured in the route.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/> instance containing the captured parameters and values.</value>
        /// <remarks>Should be empty if <see cref="IsMatch"/> is <see langword="false"/>.</remarks>
        DynamicDictionary Parameters { get; }
    }
}