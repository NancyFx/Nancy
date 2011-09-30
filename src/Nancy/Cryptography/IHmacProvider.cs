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
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        byte[] GenerateHmac(string data);

        /// <summary>
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        byte[] GenerateHmac(byte[] data);
    }
}