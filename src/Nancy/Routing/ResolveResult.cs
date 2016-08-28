namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// A class representing a route resolution result
    /// </summary>
    public class ResolveResult
    {
        /// <summary>
        /// Gets or sets the route
        /// </summary>
        public Route Route { get; set; }

        /// <summary>
        /// Gets or sets the captured parameters
        /// </summary>
        public DynamicDictionary Parameters { get; set; }

        /// <summary>
        /// Gets or sets the before module pipeline
        /// </summary>
        public BeforePipeline Before { get; set; }

        /// <summary>
        /// Gets or sets the after module pipeline
        /// </summary>
        public AfterPipeline After { get; set; }

        /// <summary>
        /// Gets or sets the on error module pipeline
        /// </summary>
        public Func<NancyContext, Exception, dynamic> OnError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveResult"/> class.
        /// </summary>
        public ResolveResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveResult"/> class.
        /// </summary>
        /// <param name="route">The request route instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="before">The before pipeline instance</param>
        /// <param name="after">The after pipeline instace.</param>
        /// <param name="onError">The on error interceptor instance.</param>
        public ResolveResult(Route route, DynamicDictionary parameters, BeforePipeline before, AfterPipeline after, Func<NancyContext, Exception, dynamic> onError)
        {
            this.Route = route;
            this.Parameters = parameters;
            this.Before = before;
            this.After = after;
            this.OnError = onError;
        }
    }
}