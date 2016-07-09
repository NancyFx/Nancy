namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="RouteConfiguration"/>.
    /// </summary>
    public class DefaultRouteConfigurationProvider : NancyDefaultConfigurationProvider<RouteConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <remarks>Will return <see cref="ViewConfiguration.Default"/>.</remarks>
        public override RouteConfiguration GetDefaultConfiguration()
        {
            return RouteConfiguration.Default;
        }
    }
}