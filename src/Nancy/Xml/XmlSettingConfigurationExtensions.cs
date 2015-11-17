namespace Nancy.Xml
{
    using System.Text;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="XmlSettings"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class XmlSettingConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="XmlSettings"/>.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="enableEncoding"><see langword="true"/> if encoding should be enabled, otherwise <see langword="false"/>.</param>
        /// <param name="defaultEncoding">The default <see cref="Encoding"/>.</param>
        public static void XmlSettings(this INancyEnvironment environment, bool enableEncoding, Encoding defaultEncoding = null)
        {
            environment.AddValue(new XmlSettings(
                enableEncoding,
                defaultEncoding));
        }
    }
}