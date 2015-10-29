namespace Nancy.Encryption.MachineKey
{
    using System;
    using System.Text;
    using System.Web.Security;

    using Nancy.Cryptography;

    /// <summary>
    /// An encryption provider that uses the ASP.Net MachineKey
    /// </summary>
    public class MachineKeyEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Encrypt and base64 encode the string
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string data)
        {
            var input = Encoding.UTF8.GetBytes(data);

            return MachineKey.Encode(input, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string data)
        {
            try
            {
                var bytes = MachineKey.Decode(data, MachineKeyProtection.Encryption);

                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
