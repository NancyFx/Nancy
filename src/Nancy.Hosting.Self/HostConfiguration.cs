namespace Nancy.Hosting.Self
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Host configuration for the self host
    /// </summary>
    public sealed class HostConfiguration
    {
        /// <summary>
        /// Gets or sets a property that determines if localhost uris are 
        /// rewritten to htp://+:port/ style uris to allow for listening on 
        /// all ports, but requiring either a url reservation, or admin
        /// access
        /// Defaults to true.
        /// </summary>
        public bool RewriteLocalhost { get; set; }

        /// <summary>
        /// Configuration around automatically creating url reservations
        /// </summary>
        public UrlReservations UrlReservations { get; set; }

        /// <summary>
        /// Gets or sets a property that determines if Transfer-Encoding: Chunked is allowed
        /// for the response instead of Content-Length (default: true).
        /// </summary>
        public bool AllowChunkedEncoding { get; set; }

        /// <summary>
        /// Gets or sets a property that provides a callback to be called
        /// if there's an unhandled exception in the self host.
        /// Note: this will *not* be called for normal nancy Exceptions
        /// that are handled by the Nancy handlers.
        /// Defaults to writing to debug out.
        /// </summary>
        public Action<Exception> UnhandledExceptionCallback { get; set; }

        /// <summary>
        /// Gets or sets a property that determines whether client certificates
        /// are enabled or not.
        /// When set to true the self host will request a client certificate if the 
        /// request is running over SSL.
        /// Defaults to false.
        /// </summary>
        public bool EnableClientCertificates { get; set; }

        /// <summary>
        /// Gets or sets a property determining if base uri matching can fall back to just
        /// using Authority (Schema + Host + Port) as base uri if it cannot match anything in
        /// the known list. This should only be set to True if you have issues with port forwarding
        /// (e.g. on Azure).
        /// </summary>
        public bool AllowAuthorityFallback { get; set; }

        public HostConfiguration()
        {
            this.RewriteLocalhost = true;
            this.UrlReservations = new UrlReservations();
            this.AllowChunkedEncoding = true;
            this.UnhandledExceptionCallback = e =>
                {
                    var message = string.Format("---\n{0}\n---\n", e);
                    Debug.Write(message);
                };
            this.EnableClientCertificates = false;
        }
    }
}
