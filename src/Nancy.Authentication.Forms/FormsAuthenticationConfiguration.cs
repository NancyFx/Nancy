namespace Nancy.Authentication.Forms
{
    using System;
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
        /// Gets or sets whether to redirect to login page during unauthorized access.
        /// </summary>
        public bool DisableRedirect { get; set; }

        /// <summary>
        /// Gets or sets the domain of the auth cookie
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the auth cookie
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Ensures the configuration is valid or not.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        public virtual void EnsureConfigurationIsValid()
        {
            if (!this.DisableRedirect && string.IsNullOrEmpty(this.RedirectUrl))
            {
                throw new InvalidOperationException("When DisableRedirect is false RedirectUrl cannot be null.");
            }

            if (this.UserMapper == null)
            {
                throw new InvalidOperationException("UserMapper cannot be null.");
            }

            if (this.CryptographyConfiguration == null)
            {
                throw new InvalidOperationException("CryptographyConfiguration cannot be null.");
            }

            if (this.CryptographyConfiguration.EncryptionProvider == null)
            {
                throw new InvalidOperationException("CryptographyConfiguration EncryptionProvider cannot be null.");
            }

            if (this.CryptographyConfiguration.HmacProvider == null)
            {
                throw new InvalidOperationException("CryptographyConfiguration HmacProvider cannot be null.");
            }
        }
    }
}
