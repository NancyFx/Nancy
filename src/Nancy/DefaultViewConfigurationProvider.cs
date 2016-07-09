namespace Nancy
{
    using Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="ViewConfiguration"/>.
    /// </summary>
    public class DefaultViewConfigurationProvider : NancyDefaultConfigurationProvider<ViewConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="ViewConfiguration.Default"/>.</remarks>
        public override ViewConfiguration GetDefaultConfiguration()
        {
            return ViewConfiguration.Default;;
        }
    }
}
