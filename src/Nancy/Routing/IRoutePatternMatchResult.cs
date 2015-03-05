namespace Nancy.Routing
{
    /// <summary>
    /// Defines the functionality that is required by a route pattern match result.
    /// </summary>
    public interface IRoutePatternMatchResult
    {
        /// <summary>
        /// Gets the <see cref="NancyContext"/> that was active when the result was produced.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        NancyContext Context { get; }

        /// <summary>
        /// Gets a value indicating whether or not a match was made.
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