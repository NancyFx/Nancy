namespace Nancy.Xml
{
    using System.Text;

    /// <summary>
    /// Configuration for XML serialization.
    /// </summary>
    public class XmlConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="XmlConfiguration"/> class.
        /// </summary>
        public static readonly XmlConfiguration Default = new XmlConfiguration
        {
            EncodingEnabled = false,
            DefaultEncoding = Encoding.UTF8
        };

        private XmlConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfiguration"/> class.
        /// </summary>
        /// <param name="encodingEnabled"><see langword="true" /> if encoding should be enabled, otherwise <see langword="false" />.</param>
        /// <param name="defaultEncoding">The <see cref="Encoding"/> that should be used.</param>
        public XmlConfiguration(bool encodingEnabled, Encoding defaultEncoding)
        {
            this.EncodingEnabled = encodingEnabled;
            this.DefaultEncoding = defaultEncoding ?? Default.DefaultEncoding;
        }

        /// <summary>
        /// Gets whether character encoding should be enabled, or not, for XML responses.
        /// </summary>
        /// <value><see langword="true" /> if encoding is enabled, otherwise <see langword="false" />.</value>
        /// <remarks>The default value is <see langword="false" />.</remarks>
        public bool EncodingEnabled { get; private set; }

        /// <summary>
        /// Gets the default encoding for XML responses.
        /// </summary>
        /// <value>The <see cref="Encoding"/> used by default.</value>
        /// <remarks>The default value is <see langword="Encoding.UTF8" />.</remarks>
        public Encoding DefaultEncoding { get; private set; }
    }
}
