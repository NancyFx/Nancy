namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="SafePathConfiguration"/>. 
    /// </summary>
    public class DefaultSafePathConfigurationProvider : NancyDefaultConfigurationProvider<SafePathConfiguration>
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Creates and instance of DefaultSafePathConfigurationProvider
        /// </summary>
        /// <param name="rootPathProvider">Use <see cref="IRootPathProvider"/> to get root path</param>
        public DefaultSafePathConfigurationProvider(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/> .
        /// </summary>
        /// <returns>The configuration instance.</returns>
        /// <remarks>Will return <see cref="SafePathConfiguration.Default"/>.</remarks>
        public override SafePathConfiguration GetDefaultConfiguration()
        {
            return new SafePathConfiguration(new []{this.rootPathProvider.GetRootPath()});
        }
    }
}

