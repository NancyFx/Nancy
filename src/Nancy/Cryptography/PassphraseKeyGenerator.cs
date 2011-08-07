namespace Nancy.Cryptography
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Provides key generation using PBKDF2 / Rfc2898
    /// NOTE: the salt is static so the passphrase should be long and complex
    /// (As the bytes are generated at app startup, because it's too slow to do per
    /// request, so the salt cannot be randomly generated and stored)
    /// </summary>
    public class PassphraseKeyGenerator : IKeyGenerator
    {
        private readonly Rfc2898DeriveBytes provider;

        public PassphraseKeyGenerator(string passphrase, byte[] salt, int iterations = 10000)
        {
            if (salt.Length < 8)
            {
                throw new ArgumentOutOfRangeException("salt", "salt must be at least 8 bytes in length");
            }

            this.provider = new Rfc2898DeriveBytes(passphrase, salt, iterations);
        }

        /// <summary>
        /// Generate a sequence of bytes
        /// </summary>
        /// <param name="count">Number of bytes to return</param>
        /// <returns>Array <see cref="count"/> bytes</returns>
        public byte[] GetBytes(int count)
        {
            return provider.GetBytes(count);
        }
    }
}