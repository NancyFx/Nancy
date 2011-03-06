namespace Nancy.Cryptography
{
    /// <summary>
    /// Creates Hash-based Message Authentication Codes (HMACs)
    /// </summary>
    public interface IHmacProvider
    {
        /// <summary>
        /// Gets the length of the HMAC signature in bytes
        /// </summary>
        int HmacLength { get; }

        /// <summary>
        /// Create a hmac from the given data using the given passPhrase
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <param name="passPhrase">Passphrase to use</param>
        /// <returns>String representation of the hmac</returns>
        byte[] GenerateHmac(string data, string passPhrase);
    }
}