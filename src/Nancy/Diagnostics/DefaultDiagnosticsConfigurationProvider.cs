namespace Nancy.Diagnostics
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="DiagnosticsConfiguration"/>.
    /// </summary>
    public class DefaultDiagnosticsConfigurationProvider : NancyDefaultConfigurationProvider<DiagnosticsConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="DiagnosticsConfiguration.Default"/></remarks>
        public override DiagnosticsConfiguration GetDefaultConfiguration()
        {
            return DiagnosticsConfiguration.Default;
        }
    }
}