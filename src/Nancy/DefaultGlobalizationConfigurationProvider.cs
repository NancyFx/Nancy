namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default <see cref="GlobalizationConfiguration"/>.
    /// </summary>
    public class DefaultGlobalizationConfigurationProvider : NancyDefaultConfigurationProvider<GlobalizationConfiguration>
    {
        /// <summary>
        /// Gets the default <see cref="GlobalizationConfiguration"/> configuration instance to register in the <see cref="INancyEnvironment"/> .
        /// </summary>
        /// <returns>The configuration instance</returns>
        public override GlobalizationConfiguration GetDefaultConfiguration()
        {
            return GlobalizationConfiguration.Default;
        }
    }
}
