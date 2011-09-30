namespace Nancy.Security
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Cryptography;

    public static class CsrfTokenExtensions
    {
        private static readonly RandomNumberGenerator randomGenerator = new RNGCryptoServiceProvider();

        /// <summary>
        /// Gets a byte array representation of the csrf token for generating
        /// hmacs
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Byte array representing the token</returns>
        public static byte[] GetCsrfTokenBytes(this CsrfToken token)
        {
            return token.RandomBytes
                        .Concat(BitConverter.GetBytes(token.CreatedDate.Ticks))
                        .ToArray();
        }

        /// <summary>
        /// Calculates and sets the Hmac property on a given token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="hmacProvider">Hmac provider to use</param>
        /// <returns>Hmac bytes</returns>
        public static void CreateHmac(this CsrfToken token, IHmacProvider hmacProvider)
        {
            token.Hmac = hmacProvider.GenerateHmac(token.GetCsrfTokenBytes());
        }

        /// <summary>
        /// Creates random bytes for the csrf token
        /// </summary>
        /// <returns>Random byte array</returns>
        public static void CreateRandomBytes(this CsrfToken token)
        {
            var randomBytes = new byte[10];

            randomGenerator.GetBytes(randomBytes);

            token.RandomBytes = randomBytes;
        }
    }
}