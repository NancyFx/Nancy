namespace Nancy.Cryptography
{
    /// <summary>
    /// Creates Hash-based Message Authentication Codes (HMACs)
    /// </summary>
    public interface IHmacProvider
    {
        /// <summary>
        /// Create a hmac from the given data using the given passPhrase
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <param name="passPhrase">Passphrase to use</param>
        /// <returns>String representation of the hmac</returns>
        string GenerateHmac(string data, string passPhrase);
    }
}