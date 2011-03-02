namespace Nancy.Session
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Encryption : IEncryption
    {
        public string Encrypt(string data, string passphrase, byte[] salt)
        {
            using (var secret = new Rfc2898DeriveBytes(passphrase, salt))
            using (var provider = new RijndaelManaged())
            using (var encryptor = provider.CreateEncryptor(secret.GetBytes(32), secret.GetBytes(16)))
            {
                byte[] input = Encoding.UTF8.GetBytes(data);
                byte[] output = encryptor.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output, 0, output.Length);
            }
        }

        public string Decrypt(string data, string passphrase, byte[] salt)
        {
            using (var secret = new Rfc2898DeriveBytes(passphrase, salt))
            using (var provider = new RijndaelManaged())
            using (var decryptor = provider.CreateDecryptor(secret.GetBytes(32), secret.GetBytes(16)))
            {
                var input = Convert.FromBase64String(data);
                var output = decryptor.TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output, 0, output.Length);
            }
        }
    }
}