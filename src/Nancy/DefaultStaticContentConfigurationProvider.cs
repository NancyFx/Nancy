namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="StaticContentConfiguration"/>. 
    /// </summary>
    public class DefaultStaticContentConfigurationProvider : NancyDefaultConfigurationProvider<StaticContentConfiguration>
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Creates and instance of DefaultStaticContentConfigurationProvider
        /// </summary>
        /// <param name="rootPathProvider">Use <see cref="IRootPathProvider"/> to get root path</param>
        public DefaultStaticContentConfigurationProvider(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/> .
        /// </summary>
        /// <returns>The configuration instance.</returns>
        public override StaticContentConfiguration GetDefaultConfiguration()
        {
            return new StaticContentConfiguration(new []{this.rootPathProvider.GetRootPath()});
        }
    }
}