namespace Nancy.Xml
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="XmlConfig"/>.
    /// </summary>
    public class XmlConfigDefaultConfigurationProvider : NancyDefaultConfigurationProvider<XmlConfig>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="XmlConfig.Default"/>.</remarks>
        public override XmlConfig GetDefaultConfiguration()
        {
            return XmlConfig.Default;
        }
    }
}
