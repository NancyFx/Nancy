namespace Nancy.Configuration
{
    /// <summary>
    /// Defines the functionality for providing default configuration values to the <see cref="INancyEnvironment"/>.
    /// </summary>
    public interface INancyDefaultConfigurationProvider
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        object GetDefaultConfiguration();
    }
}