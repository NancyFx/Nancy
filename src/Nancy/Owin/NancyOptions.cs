namespace Nancy.Owin
{
    using System;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Options for hosting Nancy with OWIN.
    /// </summary>
    public class NancyOptions
    {
        private readonly INancyBootstrapper bootstrapper;
        private Func<NancyContext, bool> performPassThrough;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyOptions"/> class.
        /// </summary>
        /// <param name="bootstrapper">A Nancy bootstrapper.</param>
        /// <exception cref="System.ArgumentNullException">bootstrapper</exception>
        public NancyOptions(INancyBootstrapper bootstrapper)
        {
            if(bootstrapper == null)
            {
                throw new ArgumentNullException("bootstrapper");
            }

            this.bootstrapper = bootstrapper;
        }

        /// <summary>
        /// Gets the bootstrapper.
        /// </summary>
        public INancyBootstrapper Bootstrapper
        {
            get { return this.bootstrapper; }
        }

        /// <summary>
        /// Gets or sets the delegate that determines if NancyMiddleware performs pass through.
        /// </summary>
        public Func<NancyContext, bool> PerformPassThrough
        {
            get { return this.performPassThrough ?? (context => false); }
            set { this.performPassThrough = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to request a client certificate or not.
        /// Defaults to false.
        /// </summary>
        public bool EnableClientCertificates { get; set; }
    }
}