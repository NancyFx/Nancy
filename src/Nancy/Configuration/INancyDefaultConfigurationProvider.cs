namespace Nancy.Configuration
{
    /// <summary>
    /// Defines the functionality for providing default configuration values to the <see cref="INancyEnvironment"/>.
    /// </summary>
    public interface INancyDefaultConfigurationProvider : IHideObjectMembers
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        object GetDefaultConfiguration();

        /// <summary>
        /// Gets the key that will be used to store the configuration object in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the key.</returns>
        string Key { get; }
    }
}