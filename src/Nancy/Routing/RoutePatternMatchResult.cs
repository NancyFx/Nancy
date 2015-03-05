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
        /// <param name="context">The <see cref="NancyContext"/> instance of the current request.</param>
        public RoutePatternMatchResult(bool isMatch, DynamicDictionary parameters, NancyContext context)
        {
            this.IsMatch = isMatch;
            this.Parameters = parameters;
            this.Context = context;
        }

        /// <summary>
        /// Gets the <see cref="NancyContext"/> that was active when the result was produced.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not a match was made.
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