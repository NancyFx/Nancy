namespace Nancy.Security
{
    using System;
    using System.Linq;
    using System.Text;
    using Cryptography;

    public static class CsrfTokenExtensions
    {
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
                        .Concat(Encoding.UTF8.GetBytes(token.Salt ?? String.Empty))
                        .ToArray();
        }

        /// <summary>
        /// Calculates and sets the Hmac property on a given token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="hmacProvider">Hmac provider to use</param>
        public static void SetHmac(this CsrfToken token, IHmacProvider hmacProvider)
        {
            token.Hmac = hmacProvider.GenerateHmac(token.GetCsrfTokenBytes());
        }
    }
}