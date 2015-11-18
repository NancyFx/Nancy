namespace Nancy.Xml
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="XmlConfiguration"/>.
    /// </summary>
    public class DefaultXmlConfigurationProvider : NancyDefaultConfigurationProvider<XmlConfiguration>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="XmlConfiguration.Default"/>.</remarks>
        public override XmlConfiguration GetDefaultConfiguration()
        {
            return XmlConfiguration.Default;
        }
    }
}
