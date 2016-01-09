namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="SafePathConfiguration"/>. 
    /// </summary>
    public class DefaultSafePathConfigurationProvider : NancyDefaultConfigurationProvider<SafePathConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/> .
        /// </summary>
        /// <returns>The configuration instance.</returns>
        /// <remarks>Will return <see cref="SafePathConfiguration.Default"/>.</remarks>
        public override SafePathConfiguration GetDefaultConfiguration()
        {
            return SafePathConfiguration.Default;
        }
    }
}

