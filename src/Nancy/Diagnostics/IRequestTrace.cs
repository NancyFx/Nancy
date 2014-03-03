namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for tracing a request.
    /// </summary>
    public interface IRequestTrace
    {
        /// <summary>
        /// Gets the HTTP verb of the request.
        /// </summary>
        /// <value>A <see cref="string"/> containg the HTTP verb.</value>
        string Method { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Url"/> that was requested.
        /// </summary>
        Url RequestUrl { get; set; }

        /// <summary>
        /// Gets the trace log.
        /// </summary>
        /// <value>A <see cref="ITraceLog"/> instance.</value>
        ITraceLog TraceLog { get; set; }

        /// <summary>
        /// Gets the generic item store.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance containing the items.</value>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the response.
        /// </summary>
        /// <value>A <see cref="Type"/> instance.</value>
        Type ResponseType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HttpStatusCode"/> of the response.
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the content type of the request.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type.</value>
        string RequestContentType { get; set; }

        /// <summary>
        /// Gets or sets the contetn type of the response.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type.</value>
        string ResponseContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers of the request.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> containing the headers.</value>
        IDictionary<string, IEnumerable<string>> RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the headers of the response.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> containing the headers.</value>
        IDictionary<string, string> ResponseHeaders { get; set; }
    }
}