namespace Nancy.Cryptography
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides SHA-1 HMACs
    /// </summary>
    public class DefaultHmacProvider : IHmacProvider
    {
        /// <summary>
        /// HMAC length
        /// </summary>
        private readonly int hmacLength = new HMACSHA256().HashSize / 8;

        /// <summary>
        /// Preferred key size for HMACSHA256
        /// </summary>
        private const int PreferredKeySize = 64;

        /// <summary>
        /// Key
        /// </summary>
        private readonly byte[] key;

        /// <summary>
        /// Creates a new instance of the DefaultHmacProvider type
        /// </summary>
        /// <param name="keyGenerator">Key generator to use to generate the key</param>
        public DefaultHmacProvider(IKeyGenerator keyGenerator)
        {
            this.key = keyGenerator.GetBytes(PreferredKeySize);
        }

        /// <summary>
        /// Gets the length of the HMAC signature
        /// </summary>
        public int HmacLength
        {
            get { return this.hmacLength; }
        }

        /// <summary>
        /// Create a hmac from the given data using the given passPhrase
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>String representation of the hmac</returns>
        public byte[] GenerateHmac(string data)
        {
            return this.GenerateHmac(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        public byte[] GenerateHmac(byte[] data)
        {
            var hmacGenerator = new HMACSHA256(this.key);

            return hmacGenerator.ComputeHash(data);
        }
    }
}