namespace Nancy.Cryptography
{
    using System.Security.Cryptography;

    /// <summary>
    /// Generates random secure keys using RandomNumberGenerator
    /// </summary>
    public class RandomKeyGenerator : IKeyGenerator
    {
        private readonly RandomNumberGenerator provider = RandomNumberGenerator.Create();

        /// <summary>
        /// Generate a sequence of bytes
        /// </summary>
        /// <param name="count">Number of bytes to return</param>
        /// <returns>
        /// Array <see paramref="count" /> bytes
        /// </returns>
        public byte[] GetBytes(int count)
        {
            var buffer = new byte[count];

            this.provider.GetBytes(buffer);

            return buffer;
        }
    }
}
