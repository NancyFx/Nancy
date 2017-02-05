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

        /// <summary>
        /// Gets or sets a property determining how many total connections the NancyHost can maintain simultaneously.
        /// Higher values mean more conections can be maintained at a slower average response times; while fewer connections will be rejected.
        /// Lower values will result in fewer conections, yet will be maintained at a faster average response time.
        /// </summary>
        public int MaximumConnectionCount { get; set; }


        /// <summary>
        /// Gets approximate processor thread count by halfing the Logical Core count to 
        /// account for hyper-threading.
        /// </summary>
        private static int ProcessorThreadCount
        {
            get
            {
                // Divide by 2 for hyper-threading, and good defaults.
                var threadCount = Environment.ProcessorCount >> 1;

                if (threadCount < 1)
                {
                    // Ensure thread count is at least 1.
                    return 1;
                }

                return threadCount;
            }
        }

        /// <summary>
        /// Initializes the default configuration.
        /// MaximumConnectionCount by default is half of the Logical Core count.
        /// </summary>
        /// <remarks>
        /// If the system running NancyHost is not using hyper-threading you may want to consider
        /// supplying your own values for MaximumConnectionCount as the default assumes 
        /// hyperthreading is being utilized.
        /// </remarks>
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
            this.MaximumConnectionCount = ProcessorThreadCount;
        }


    }
}
