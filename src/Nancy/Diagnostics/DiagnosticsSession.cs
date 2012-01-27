namespace Nancy.Diagnostics
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    [Serializable]
    public class DiagnosticsSession
    {
        public byte[] Hash { get; set; }

        public byte[] Salt { get; set; }

        public DateTime Expiry { get; set; }

        public static byte[] GenerateRandomSalt()
        {
            var provider = new RNGCryptoServiceProvider();

            var buffer = new byte[32];
            provider.GetBytes(buffer);

            return buffer;
        }

        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var algorithm = new SHA256Managed();

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

        public static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            return GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), salt);
        }
    }
}