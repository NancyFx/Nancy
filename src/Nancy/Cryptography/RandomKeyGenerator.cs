namespace Nancy.Cryptography
{
    using System.Security.Cryptography;

    /// <summary>
    /// Generates random secure keys using RNGCryptoServiceProvider
    /// </summary>
    public class RandomKeyGenerator : IKeyGenerator
    {
        private readonly RandomNumberGenerator provider = RandomNumberGenerator.Create();

        public byte[] GetBytes(int count)
        {
            var buffer = new byte[count];

            this.provider.GetBytes(buffer);

            return buffer;
        }
    }
}