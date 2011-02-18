namespace Nancy.Routing
{
    /// <summary>
    /// The default implementation of a route pattern matching result.
    /// </summary>
    public class RoutePatternMatchResult : IRoutePatternMatchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePatternMatchResult"/> class.
        /// </summary>
        /// <param name="isMatch">A <see cref="bool"/> value indicating if the result was a match or not.</param>
        /// <param name="parameters">A <see cref="DynamicDictionary"/> instance containing the parameters and values that was captured in the match.</param>
        public RoutePatternMatchResult(bool isMatch, DynamicDictionary parameters)
        {
            this.IsMatch = isMatch;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets a value idicating wether or not a match was made.
        /// </summary>
        /// <value><see langword="true"/> if a match was made; otherwise <see langword="false"/>.</value>
        public bool IsMatch { get; private set; }

        /// <summary>
        /// The parameters that could be captured in the route.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/> instance containing the captured parameters and values.</value>
        /// <remarks>Should be empty if <see cref="IRoutePatternMatchResult.IsMatch"/> is <see langword="false"/>.</remarks>
        public DynamicDictionary Parameters { get; private set; }
    }
}