namespace Nancy.Session
{
    using Nancy.Cryptography;

    /// <summary>
    /// Configuration options for cookie based sessions
    /// </summary>
    public class CookieBasedSessionsConfiguration
    {
        internal const string DefaultCookieName = "_nc";

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessionsConfiguration"/> class.
        /// </summary>
        public CookieBasedSessionsConfiguration() : this(CryptographyConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessionsConfiguration"/> class.
        /// </summary>
        public CookieBasedSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration)
        {
            CryptographyConfiguration = cryptographyConfiguration;
            CookieName = DefaultCookieName;
        }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Formatter for de/serializing the session objects
        /// </summary>
        public IObjectSerializer Serializer { get; set; }

        /// <summary>
        /// Cookie name for storing session information
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the domain of the session cookie
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the session cookie
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(this.CookieName))
                {
                    return false;
                }

                if (this.Serializer == null)
                {
                    return false;
                }

                if (this.CryptographyConfiguration == null)
                {
                    return false;
                }

                if (this.CryptographyConfiguration.EncryptionProvider == null)
                {
                    return false;
                }

                if (this.CryptographyConfiguration.HmacProvider == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
