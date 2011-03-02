namespace Nancy.Session
{
    public interface IEncryption
    {
        string Encrypt(string data, string passphrase, byte[] salt);
        string Decrypt(string data, string passphrase, byte[] salt);
    }
}