namespace Nancy.Xml
{
    using System.Text;

    public static class XmlSettings
    {
        static XmlSettings()
        {
            EncodingEnabled = false;
            DefaultEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Gets or sets whether character encoding should be enabled, or not, for XML responses.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="false" />
        /// </remarks>
        public static bool EncodingEnabled { get; set; }

        /// <summary>
        /// Gets the default encoding for XML responses.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="Encoding.UTF8" />
        /// </remarks>
        public static Encoding DefaultEncoding { get; set; }
    }
}