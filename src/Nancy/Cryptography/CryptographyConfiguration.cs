namespace Nancy.Cryptography
{
    /// <summary>
    /// Cryptographic setup for classes that use encryption and HMAC
    /// </summary>
    public class CryptographyConfiguration
    {
        /// <summary>
        /// Initializes static members of the <see cref="CryptographyConfiguration"/> class.
        /// </summary>
        static CryptographyConfiguration()
        {
            Default = new CryptographyConfiguration(
                    new RijndaelEncryptionProvider(new RandomKeyGenerator()),
                    new DefaultHmacProvider(new RandomKeyGenerator()));

            NoEncryption = new CryptographyConfiguration(
                    new NoEncryptionProvider(),
                    new DefaultHmacProvider(new RandomKeyGenerator()));
        }

        /// <summary>
        /// Creates a new instance of the CryptographyConfiguration class
        /// </summary>
        /// <param name="encryptionProvider">Encryption provider</param>
        /// <param name="hmacProvider">HMAC provider</param>
        public CryptographyConfiguration(IEncryptionProvider encryptionProvider, IHmacProvider hmacProvider)
        {
            this.EncryptionProvider = encryptionProvider;
            this.HmacProvider = hmacProvider;
        }

        /// <summary>
        /// Gets the default configuration - Rijndael encryption, HMACSHA256 HMAC, random keys
        /// </summary>
        public static CryptographyConfiguration Default { get; private set; }

        /// <summary>
        /// Gets configuration with no encryption and HMACSHA256 HMAC with a random key
        /// </summary>
        public static CryptographyConfiguration NoEncryption { get; private set; }

        /// <summary>
        /// Gets the encryption provider
        /// </summary>
        public IEncryptionProvider EncryptionProvider { get; private set; }

        /// <summary>
        /// Gets the hmac provider
        /// </summary>
        public IHmacProvider HmacProvider { get; private set; }
    }
}