namespace Nancy.Cryptography
{
    using System;
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
        private readonly int hmacLength = new HMACSHA1().HashSize / 8;

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
        /// <param name="passPhrase">Passphrase to use</param>
        /// <returns>String representation of the hmac</returns>
        public byte[] GenerateHmac(string data, string passPhrase)
        {
            var hmacGenerator = new HMACSHA1(Encoding.UTF8.GetBytes(passPhrase));

            return hmacGenerator.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
    }
}