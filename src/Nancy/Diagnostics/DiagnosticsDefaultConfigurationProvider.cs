namespace Nancy.Diagnostics
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for diagnostics.
    /// </summary>
    public class DiagnosticsDefaultConfigurationProvider : INancyDefaultConfigurationProvider
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="DiagnosticsConfiguration.Default"/></remarks>
        public object GetDefaultConfiguration()
        {
            return DiagnosticsConfiguration.Default;
        }
    }
}