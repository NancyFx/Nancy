namespace Nancy.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Default encryption provider using Aes
    /// </summary>
    public class AesEncryptionProvider : IEncryptionProvider
    {
        private readonly byte[] key;

        private readonly byte[] iv;

        /// <summary>
        /// Creates a new instance of the AesEncryptionProvider class
        /// </summary>
        /// <param name="keyGenerator">Key generator to use to generate the key and iv</param>
        public AesEncryptionProvider(IKeyGenerator keyGenerator)
        {
            this.key = keyGenerator.GetBytes(32);
            this.iv = keyGenerator.GetBytes(16);
        }

        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string data)
        {
            using (var provider = Aes.Create())
            using (var encryptor = provider.CreateEncryptor(this.key, this.iv))
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
        /// <returns>Decrypted string</returns>
        public string Decrypt(string data)
        {
            try
            {
                using (var provider = Aes.Create())
                using (var decryptor = provider.CreateDecryptor(this.key, this.iv))
                {
                    var input = Convert.FromBase64String(data);
                    var output = decryptor.TransformFinalBlock(input, 0, input.Length);

                    return Encoding.UTF8.GetString(output);
                }
            }
            catch (FormatException)
            {
                return String.Empty;
            }
            catch (CryptographicException)
            {
                return String.Empty;
            }
            catch(ArgumentException ex)
            {
                if (ex.ParamName == null)
                {
                    return String.Empty;
                }
                throw ex;
            }
        }
    }
}
