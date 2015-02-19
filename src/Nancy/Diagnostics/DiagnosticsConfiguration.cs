namespace Nancy.Diagnostics
{
    using Nancy.Cryptography;

    /// <summary>
    /// Settings for the diagnostics dashboard
    /// </summary>
    public class DiagnosticsConfiguration
    {
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsConfiguration"/> class,
        /// using the <see cref="CryptographyConfiguration.Default"/> cryptographic
        /// configuration.
        /// </summary>
        public DiagnosticsConfiguration()
            : this(CryptographyConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsConfiguration"/> class,
        /// using the <paramref name="cryptographyConfiguration"/> cryptographic
        /// configuration.
        /// </summary>
        /// <param name="cryptographyConfiguration">The <see cref="CryptographyConfiguration"/> to use with diagnostics.</param>
        public DiagnosticsConfiguration(CryptographyConfiguration cryptographyConfiguration)
        {
            this.CookieName = "__ncd";
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.Path = "/_Nancy";
            this.SlidingTimeout = 15;
        }

        /// <summary>
        /// Gets or sets the name of the cookie used by the diagnostics dashboard.
        /// </summary>
        /// <remarks>The default is __ncd</remarks>
        public string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the cryptography config to use for securing the diagnostics dashboard
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Gets or sets password for accessing the diagnostics screen.
        /// This should be secure :-)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the path that the diagnostics dashboard will be accessible on.
        /// </summary>
        /// <remarks>The default is /_Nancy. The path should always start with a forward slash.</remarks>
        public string Path
        {
            get { return this.path; }
            set { this.path = (!value.StartsWith("/")) ? string.Concat("/", value) : value; }
        }

        /// <summary>
        /// The number of minutes that expiry of the diagnostics dashboard will be extended each time it is used.
        /// </summary>
        /// <remarks>The default is 15 minutes.</remarks>
        public int SlidingTimeout { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid
        /// </summary>
        public bool Valid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Password) &&
                    !string.IsNullOrWhiteSpace(this.CookieName) &&
                    !string.IsNullOrWhiteSpace(this.Path) &&
                    this.SlidingTimeout != 0;
            }
        }
    }
}
