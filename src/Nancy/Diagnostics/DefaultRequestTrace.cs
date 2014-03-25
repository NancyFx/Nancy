namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    /// <summary>
    /// The default implementation of the <see cref="IRequestTrace"/> interface.
    /// </summary>
    public class DefaultRequestTrace : IRequestTrace
    {
        /// <summary>
        /// Gets the generic item store.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance containing the items.</value>
        public IDictionary<string, object> Items { get; set; }

        /// <summary>
        /// Gets or sets the information about the request.
        /// </summary>
        /// <value>An <see cref="RequestData"/> instance.</value>
        public RequestData RequestData { get; set; }

        /// <summary>
        /// Gets or sets the information about the response.
        /// </summary>
        /// <value>An <see cref="ResponseData"/> instance.</value>
        public ResponseData ResponseData { get; set; }

        /// <summary>
        /// Gets or sets the trace log.
        /// </summary>
        /// <value>A <see cref="ITraceLog"/> instance.</value>
        public ITraceLog TraceLog { get; set; }
    }
}