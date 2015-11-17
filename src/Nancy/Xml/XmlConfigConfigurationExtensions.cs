namespace Nancy.Xml
{
    using System.Text;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="XmlConfig"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class XmlConfigConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="XmlConfig"/>.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="enableEncoding"><see langword="true"/> if encoding should be enabled, otherwise <see langword="false"/>.</param>
        /// <param name="defaultEncoding">The default <see cref="Encoding"/>.</param>
        public static void XmlConfig(this INancyEnvironment environment, bool enableEncoding, Encoding defaultEncoding = null)
        {
            environment.AddValue(new XmlConfig(
                enableEncoding,
                defaultEncoding));
        }
    }
}
