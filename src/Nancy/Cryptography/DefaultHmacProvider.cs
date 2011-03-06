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
        /// Create a hmac from the given data using the given passPhrase
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <param name="passPhrase">Passphrase to use</param>
        /// <returns>String representation of the hmac</returns>
        public string GenerateHmac(string data, string passPhrase)
        {
            var hmacGenerator = new HMACSHA1(Encoding.UTF8.GetBytes(passPhrase));

            var hmacBytes = hmacGenerator.ComputeHash(Encoding.UTF8.GetBytes(data));

            return Convert.ToBase64String(hmacBytes);
        }
    }
}