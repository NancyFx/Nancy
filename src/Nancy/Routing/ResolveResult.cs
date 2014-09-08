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
        public Func<NancyContext, Exception, Response> OnError { get; set; }

        public ResolveResult()
        {
        }

        public ResolveResult(Route route, DynamicDictionary parameters, BeforePipeline before, AfterPipeline after, Func<NancyContext, Exception, Response> onError)
        {
            this.Route = route;
            this.Parameters = parameters;
            this.Before = before;
            this.After = after;
            this.OnError = onError;
        }
    }
}