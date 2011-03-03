namespace Nancy.Session
{
    /// <summary>
    /// Provides symetrical encryption support
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Encyrypt data
        /// </summary>
        /// <param name="data">Data to encrypyt</param>
        /// <param name="passphrase">Passphrase to use</param>
        /// <param name="salt">Salt to use</param>
        /// <returns>Encrypted string</returns>
        string Encrypt(string data, string passphrase, byte[] salt);

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="passphrase">Passphrase to use</param>
        /// <param name="salt">Salt to use</param>
        /// <returns>Decrypted string</returns>
        string Decrypt(string data, string passphrase, byte[] salt);
    }
}