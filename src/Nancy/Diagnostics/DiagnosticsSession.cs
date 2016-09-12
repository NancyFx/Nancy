namespace Nancy.Diagnostics
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    
    /// <summary>
    /// Stores the http session information for diagnostics.
    /// </summary>
    
#if !NETSTANDARD1_6
    [Serializable]
#endif

    public class DiagnosticsSession
    {
        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The (salted) SHA256 hash.</value>
        public byte[] Hash { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>The salt for the hash value.</value>
        public byte[] Salt { get; set; }

        /// <summary>
        /// Gets or sets the expiry.
        /// </summary>
        /// <value>The time when the session will be expired.</value>
        public DateTimeOffset Expiry { get; set; }

        /// <summary>
        /// Generates a random salt.
        /// </summary>
        /// <returns>A byte array representing the random salt.</returns>
        public static byte[] GenerateRandomSalt()
        {
            var provider = RandomNumberGenerator.Create();

            var buffer = new byte[32];
            provider.GetBytes(buffer);

            return buffer;
        }

        /// <summary>
        /// Generates the salted hash of a byte array.
        /// </summary>
        /// <param name="plainText">The plain text as <see cref="byte"/> array.</param>
        /// <param name="salt">The salt as <see cref="byte"/> array.</param>
        /// <returns>A byte array representing the salted hash.</returns>
        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var algorithm = SHA256.Create();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Generates the salted hash of a <see cref="string"/>.
        /// </summary>
        /// <param name="plainText">The plain text as <see cref="string"/></param>
        /// <param name="salt">The salt as <see cref="byte"/> array.</param>
        /// <returns>A byte array representing the salted hash.</returns>
        public static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            return GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), salt);
        }
    }
}