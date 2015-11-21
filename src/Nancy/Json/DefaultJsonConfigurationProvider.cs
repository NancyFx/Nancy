namespace Nancy.Json
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="JsonConfiguration"/>.
    /// </summary>
    public class DefaultJsonConfigurationProvider : NancyDefaultConfigurationProvider<JsonConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="JsonConfiguration.Default"/></remarks>
        public override JsonConfiguration GetDefaultConfiguration()
        {
            return JsonConfiguration.Default;
        }
    }
}