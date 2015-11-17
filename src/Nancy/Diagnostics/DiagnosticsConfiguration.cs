namespace Nancy.Diagnostics
{
    using Nancy.Cryptography;

    /// <summary>
    /// Configuration for the diagnostics dashboard.
    /// </summary>
    public class DiagnosticsConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="DiagnosticsConfiguration"/> class.
        /// </summary>
        public static readonly DiagnosticsConfiguration Default = new DiagnosticsConfiguration
        {
            CookieName = "__ncd",
            CryptographyConfiguration = CryptographyConfiguration.Default,
            Password = null,
            Path = "/_Nancy",
            SlidingTimeout = 15
        };

        private DiagnosticsConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsConfiguration"/> class
        /// </summary>
        /// <param name="password">Password used to secure the dashboard.</param>
        /// <param name="path">Relative path of the dashboard.</param>
        /// <param name="cookieName">Name of the cookie to store diagnostics information.</param>
        /// <param name="slidingTimeout">Number of minutes that expiry of the diagnostics dashboard.</param>
        /// <param name="cryptographyConfiguration">Cryptography config to use for securing the dashboard.</param>
        public DiagnosticsConfiguration(string password, string path, string cookieName, int slidingTimeout, CryptographyConfiguration cryptographyConfiguration)
        {
            this.Password = password ?? Default.Password;
            this.Path = GetNormalizedPath(path ?? Default.Path);
            this.CookieName = cookieName ?? Default.CookieName;
            this.SlidingTimeout = slidingTimeout;
            this.CryptographyConfiguration = cryptographyConfiguration ?? Default.CryptographyConfiguration;
        }

        /// <summary>
        /// Gets or sets the name of the cookie used by the diagnostics dashboard.
        /// </summary>
        /// <remarks>The default is __ncd</remarks>
        public string CookieName { get; private set; }

        /// <summary>
        /// Gets or sets the cryptography config to use for securing the diagnostics dashboard
        /// </summary>
        /// <remarks>The default is <see cref="CryptographyConfiguration.Default"/></remarks>
        public CryptographyConfiguration CryptographyConfiguration { get; private set; }

        /// <summary>
        /// Gets or sets password for accessing the diagnostics screen.
        /// </summary>
        /// <remarks>The default value is <see langword="null" />.</remarks>
        public string Password { get; private set; }

        /// <summary>
        /// Gets or sets the path that the diagnostics dashboard will be accessible on.
        /// </summary>
        /// <remarks>The default is /_Nancy.</remarks>
        public string Path { get; private set; }

        /// <summary>
        /// The number of minutes that expiry of the diagnostics dashboard. Will be extended each time it is used.
        /// </summary>
        /// <remarks>The default is 15 minutes.</remarks>
        public int SlidingTimeout { get; private set; }

        private static string GetNormalizedPath(string path)
        {
            return (!path.StartsWith("/")) ? string.Concat("/", path) : path;
        }
    }
}
