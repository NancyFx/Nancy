namespace Nancy.Owin
{
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    public class NancyOptions
    {
        private INancyBootstrapper bootstrapper;
        private IEnumerable<HttpStatusCode> passThroughStatusCodes;

        /// <summary>
        /// Gets or sets the bootstrapper. If none is set, NancyBootstrapperLocator.Bootstrapper is used.
        /// </summary>
        public INancyBootstrapper Bootstrapper
        {
            get { return this.bootstrapper ?? NancyBootstrapperLocator.Bootstrapper; }
            set { this.bootstrapper = value; }
        }

        /// <summary>
        /// Gets or sets the collection of pass through status codes.
        /// </summary>
        public IEnumerable<HttpStatusCode> PassThroughStatusCodes
        {
            get { return this.passThroughStatusCodes ?? Enumerable.Empty<HttpStatusCode>(); }
            set { this.passThroughStatusCodes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to request a client certificate or not.
        /// Defaults to false.
        /// </summary>
        public bool EnableClientCertificates { get; set; }
    }
}