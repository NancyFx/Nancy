namespace Nancy.Cryptography
{
    using System.Security.Cryptography;

    /// <summary>
    /// Provides key generation using PBKDF2 / Rfc2898
    /// NOTE: this is *not* salted so the password should be long and complicated
    /// (As the bytes are generated at app startup, because it's too slow to do per
    /// request, salting has no benefit)
    /// </summary>
    public class PassphraseKeyGenerator : IKeyGenerator
    {
        private readonly Rfc2898DeriveBytes provider;

        public PassphraseKeyGenerator(string passphrase)
        {
            this.provider = new Rfc2898DeriveBytes(passphrase, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
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