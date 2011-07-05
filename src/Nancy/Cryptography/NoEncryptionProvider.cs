namespace Nancy.Cryptography
{
    using System;
    using System.Text;

    /// <summary>
    /// A "no op" encryption provider
    /// Useful for debugging or performance.
    /// </summary>
    public class NoEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Encyrypt data
        /// </summary>
        /// <param name="data">Data to encrypyt</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }
}