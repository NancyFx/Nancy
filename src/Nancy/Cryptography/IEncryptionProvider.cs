namespace Nancy.Cryptography
{
    /// <summary>
    /// Provides symmetrical encryption support
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Encrypt and base64 encode the string
        /// </summary>
        /// <param name="data">Data to encrypyt</param>
        /// <returns>Encrypted string</returns>
        string Encrypt(string data);

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <returns>Decrypted string</returns>
        string Decrypt(string data);
    }
}