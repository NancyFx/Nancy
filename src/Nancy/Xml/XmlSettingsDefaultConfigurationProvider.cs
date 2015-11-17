namespace Nancy.Xml
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="XmlSettings"/>.
    /// </summary>
    public class XmlSettingsDefaultConfigurationProvider : NancyDefaultConfigurationProvider<XmlSettings>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        /// <remarks>Will return <see cref="XmlSettings.Default"/>.</remarks>
        public override XmlSettings GetDefaultConfiguration()
        {
            return XmlSettings.Default;
        }
    }
}