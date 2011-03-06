namespace Nancy.Authentication.Forms
{
    using System.Text;
    using Cryptography;

    /// <summary>
    /// Configuration options for forms authentication
    /// </summary>
    public class FormsAuthenticationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormsAuthenticationConfiguration"/> class.
        /// </summary>
        public FormsAuthenticationConfiguration()
        {
            this.EncryptionProvider = new DefaultEncryptionProvider();
            this.HmacProvider = new DefaultHmacProvider();
        }

        /// <summary>
        /// Gets or sets the passphrase for encrypting the forms authentication cookie
        /// </summary>
        public string Passphrase { get; set; }

        /// <summary>
        /// Gets or sets the salt for encrypting the forms authentication cookie
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Gets the salt as a byte array
        /// </summary>
        public byte[] SaltBytes
        {
            get { return Encoding.UTF8.GetBytes(this.Salt); }
        }

        /// <summary>
        /// Gets or sets the passphrase for signing forms authentication cookie
        /// </summary>
        public string HmacPassphrase { get; set; }

        /// <summary>
        /// Gets or sets the redirect url for pages that require authentication
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the username/identifier mapper
        /// </summary>
        public IUsernameMapper UsernameMapper { get; set; }

        /// <summary>
        /// Gets or sets the encryption provider
        /// </summary>
        public IEncryptionProvider EncryptionProvider { get; set; }

        /// <summary>
        /// Gets or sets the hmac provider
        /// </summary>
        public IHmacProvider HmacProvider { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(this.Passphrase))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(this.Salt) || this.Salt.Length < 8)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(this.HmacPassphrase))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(this.RedirectUrl))
                {
                    return false;
                }

                if (this.UsernameMapper == null)
                {
                    return false;
                }

                if (this.EncryptionProvider == null)
                {
                    return false;
                }

                if (this.HmacProvider == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}