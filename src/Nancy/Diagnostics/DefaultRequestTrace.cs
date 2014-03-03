namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The default implementation of the <see cref="IRequestTrace"/> interface.
    /// </summary>
    public class DefaultRequestTrace : IRequestTrace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestTrace"/> class.
        /// </summary>
        public DefaultRequestTrace()
            : this(StaticConfiguration.EnableRequestTracing)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestTrace"/> class.
        /// </summary>
        /// <param name="logActive"><see langword="true"/> if trace logging should be enabled; otherwise <see langword="false" />.</param>
        public DefaultRequestTrace(bool logActive)
        {
            this.TraceLog = logActive ? (ITraceLog)new TraceLog() : new NullLog();
            this.Items = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the HTTP verb of the request.
        /// </summary>
        /// <value>A <see cref="string"/> containg the HTTP verb.</value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Url"/> that was requested.
        /// </summary>
        public Url RequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the trace log.
        /// </summary>
        /// <value>A <see cref="ITraceLog"/> instance.</value>
        public ITraceLog TraceLog { get; set; }

        /// <summary>
        /// Gets the generic item store.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance containing the items.</value>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the response.
        /// </summary>
        /// <value>A <see cref="Type"/> instance.</value>
        public Type ResponseType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HttpStatusCode"/> of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the content type of the request.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type.</value>
        public string RequestContentType { get; set; }

        /// <summary>
        /// Gets or sets the contetn type of the response.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type.</value>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers of the request.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> containing the headers.</value>
        public IDictionary<string, IEnumerable<string>> RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the headers of the response.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> containing the headers.</value>
        public IDictionary<string, string> ResponseHeaders { get; set; }
    }
}