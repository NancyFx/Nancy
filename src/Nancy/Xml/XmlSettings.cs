namespace Nancy.Xml
{
    using System.Text;
    using Nancy.Configuration;

    /// <summary>
    ///
    /// </summary>
    public class XmlSettings
    {
        /// <summary>
        ///
        /// </summary>
        public static readonly XmlSettings Default = new XmlSettings
        (
            encodingEnabled: false,
            defaultEncoding: Encoding.UTF8
        );

        /// <summary>
        ///
        /// </summary>
        /// <param name="encodingEnabled"></param>
        /// <param name="defaultEncoding"></param>
        public XmlSettings(bool encodingEnabled, Encoding defaultEncoding)
        {
            this.EncodingEnabled = encodingEnabled;
            this.DefaultEncoding = defaultEncoding ?? Default.DefaultEncoding;
        }

        /// <summary>
        /// Gets whether character encoding should be enabled, or not, for XML responses.
        /// </summary>
        /// <remarks>The default value is <see langword="false" />.</remarks>
        public bool EncodingEnabled { get; private set; }

        /// <summary>
        /// Gets the default encoding for XML responses.
        /// </summary>
        /// <remarks>The default value is <see langword="Encoding.UTF8" />.</remarks>
        public Encoding DefaultEncoding { get; private set; }
    }

    /// <summary>
    ///
    /// </summary>
    public static class XmlSettingConfigurationExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="enableEncoding"></param>
        /// <param name="defaultEncoding"></param>
        public static void XmlSettings(this INancyEnvironment environment, bool enableEncoding, Encoding defaultEncoding = null)
        {
            environment.AddValue(new XmlSettings(
                enableEncoding,
                defaultEncoding));
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class XmlSettingsDefaultConfigurationProvider : NancyDefaultConfigurationProvider<XmlSettings>
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        public override XmlSettings GetDefaultConfiguration()
        {
            return XmlSettings.Default;
        }
    }
}