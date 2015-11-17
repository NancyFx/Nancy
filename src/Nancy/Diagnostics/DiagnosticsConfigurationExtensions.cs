namespace Nancy.Diagnostics
{
    using Nancy.Configuration;
    using Nancy.Cryptography;

    /// <summary>
    /// Contains <see cref="DiagnosticsConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class DiagnosticsConfigurationExtensions
    {
        /// <summary>
        /// Configures diagnostics.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="password">Password used to secure the dashboard.</param>
        /// <param name="path">Relative path of the dashboard.</param>
        /// <param name="cookieName">Name of the cookie to store diagnostics information.</param>
        /// <param name="slidingTimeout">Number of minutes that expiry of the diagnostics dashboard.</param>
        /// <param name="cryptographyConfiguration">Cryptography config to use for securing the dashboard.</param>
        public static void Diagnostics(this INancyEnvironment environment, string password, string path = null, string cookieName = null, int slidingTimeout = 15, CryptographyConfiguration cryptographyConfiguration = null)
        {
            environment.AddValue(new DiagnosticsConfiguration(
                password,
                path,
                cookieName,
                slidingTimeout,
                cryptographyConfiguration));
        }
    }
}