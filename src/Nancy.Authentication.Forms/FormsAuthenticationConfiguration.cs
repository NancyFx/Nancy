namespace Nancy.Authentication.Forms
{
    using Cryptography;

    /// <summary>
    /// Configuration options for forms authentication
    /// </summary>
    public class FormsAuthenticationConfiguration
    {
        internal const string DefaultRedirectQuerystringKey = "returnUrl";

        /// <summary>
        /// Initializes a new instance of the <see cref="FormsAuthenticationConfiguration"/> class.
        /// </summary>
        public FormsAuthenticationConfiguration() : this(CryptographyConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormsAuthenticationConfiguration"/> class.
        /// </summary>
        /// <param name="cryptographyConfiguration">Cryptography configuration</param>
        public FormsAuthenticationConfiguration(CryptographyConfiguration cryptographyConfiguration)
        {
            CryptographyConfiguration = cryptographyConfiguration;
            RedirectQuerystringKey = DefaultRedirectQuerystringKey;
        }

        /// <summary>
        /// Gets or sets the forms authentication query string key for storing the return url
        /// </summary>
        public string RedirectQuerystringKey { get; set; }

        /// <summary>
        /// Gets or sets the redirect url for pages that require authentication
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the username/identifier mapper
        /// </summary>
        public IUserMapper UserMapper { get; set; }

        /// <summary>
        /// Gets or sets RequiresSSL property
        /// </summary>
        /// <value>The flag that indicates whether SSL is required</value>
        public bool RequiresSSL { get; set; }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(this.RedirectUrl))
                {
                    return false;
                }

                if (this.UserMapper == null)
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