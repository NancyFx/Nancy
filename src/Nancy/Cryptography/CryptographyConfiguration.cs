namespace Nancy.Cryptography
{
    using System;

    /// <summary>
    /// Cryptographic setup for classes that use encryption and HMAC
    /// </summary>
    public class CryptographyConfiguration
    {
        private static readonly Lazy<CryptographyConfiguration> DefaultConfiguration =
            new Lazy<CryptographyConfiguration>(() => new CryptographyConfiguration(
                                                          new AesEncryptionProvider(new RandomKeyGenerator()),
                                                          new DefaultHmacProvider(new RandomKeyGenerator())));

        private static readonly Lazy<CryptographyConfiguration> NoEncryptionConfiguration =
            new Lazy<CryptographyConfiguration>(() => new CryptographyConfiguration(
                                                          new NoEncryptionProvider(),
                                                          new DefaultHmacProvider(new RandomKeyGenerator())));

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
        public static CryptographyConfiguration Default
        {
            get { return DefaultConfiguration.Value; }
        }

        /// <summary>
        /// Gets configuration with no encryption and HMACSHA256 HMAC with a random key
        /// </summary>
        public static CryptographyConfiguration NoEncryption
        {
            get { return NoEncryptionConfiguration.Value; }
        }

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