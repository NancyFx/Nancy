namespace Nancy.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Default encryption provider using Rijndael
    /// </summary>
    public class DefaultEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Encyrypt data
        /// </summary>
        /// <param name="data">Data to encrypyt</param>
        /// <param name="passphrase">Passphrase to use</param>
        /// <param name="salt">Salt to use</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string data, string passphrase, byte[] salt)
        {
            using (var secret = new Rfc2898DeriveBytes(passphrase, salt))
            using (var provider = new RijndaelManaged())
            using (var encryptor = provider.CreateEncryptor(secret.GetBytes(32), secret.GetBytes(16)))
            {
                var input = Encoding.UTF8.GetBytes(data);
                var output = encryptor.TransformFinalBlock(input, 0, input.Length);

                return Convert.ToBase64String(output);
            }
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="passphrase">Passphrase to use</param>
        /// <param name="salt">Salt to use</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string data, string passphrase, byte[] salt)
        {
            try
            {
                using (var secret = new Rfc2898DeriveBytes(passphrase, salt))
                using (var provider = new RijndaelManaged())
                using (var decryptor = provider.CreateDecryptor(secret.GetBytes(32), secret.GetBytes(16)))
                {
                    var input = Convert.FromBase64String(data);
                    var output = decryptor.TransformFinalBlock(input, 0, input.Length);

                    return Encoding.UTF8.GetString(output);
                }
            }
            catch (CryptographicException)
            {
                return String.Empty;
            }
        }
    }
}