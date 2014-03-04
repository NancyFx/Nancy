namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for tracing a request.
    /// </summary>
    public interface IRequestTrace
    {
        /// <summary>
        /// Gets or sets the generic item store.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance containing the items.</value>
        IDictionary<string, object> Items { get; set; }

        /// <summary>
        /// Gets or sets the information about the request.
        /// </summary>
        /// <value>An <see cref="RequestData"/> instance.</value>
        RequestData RequestData { get; set; }

        /// <summary>
        /// Gets or sets the information about the response.
        /// </summary>
        /// <value>An <see cref="ResponseData"/> instance.</value>
        ResponseData ResponseData { get; set; }

        /// <summary>
        /// Gets the trace log.
        /// </summary>
        /// <value>A <see cref="ITraceLog"/> instance.</value>
        ITraceLog TraceLog { get; set; }
    }
}