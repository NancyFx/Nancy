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
        /// <remarks>This will implicitly enable diagnostics. If you need control, please explicitly set enabled to either <see langword="true"/> or <see langword="false"/>.</remarks>
        public static void Diagnostics(this INancyEnvironment environment, string password, string path = null, string cookieName = null, int slidingTimeout = 15, CryptographyConfiguration cryptographyConfiguration = null)
        {
            Diagnostics(
                environment,
                enabled: true,
                password: password,
                path: path,
                cookieName: cookieName,
                slidingTimeout: slidingTimeout,
                cryptographyConfiguration: cryptographyConfiguration);
        }

        /// <summary>
        /// Configures diagnostics.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="enabled"><see langword="true"/> if diagnostics should be enabled, otherwise <see langword="false"/>.</param>
        /// <param name="password">Password used to secure the dashboard.</param>
        /// <param name="path">Relative path of the dashboard.</param>
        /// <param name="cookieName">Name of the cookie to store diagnostics information.</param>
        /// <param name="slidingTimeout">Number of minutes that expiry of the diagnostics dashboard.</param>
        /// <param name="cryptographyConfiguration">Cryptography config to use for securing the dashboard.</param>
        public static void Diagnostics(this INancyEnvironment environment, bool enabled, string password, string path = null, string cookieName = null, int slidingTimeout = 15, CryptographyConfiguration cryptographyConfiguration = null)
        {
            environment.AddValue(new DiagnosticsConfiguration(
                enabled: enabled,
                password: password,
                path: path,
                cookieName: cookieName,
                slidingTimeout: slidingTimeout,
                cryptographyConfiguration: cryptographyConfiguration));
        }
    }
}
