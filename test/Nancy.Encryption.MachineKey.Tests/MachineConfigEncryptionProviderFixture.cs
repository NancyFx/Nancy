namespace Nancy.Encryption.MachineKey.Tests
{
    using Nancy.Cryptography;
    using Nancy.Tests;

    using Xunit;

    public class MachineConfigEncryptionProviderFixture
    {
        private readonly IEncryptionProvider provider = new MachineKeyEncryptionProvider();

        [Fact]
        public void Should_encrypt_data()
        {
            const string input = "This is some input";

            var output = provider.Encrypt(input);

            output.ShouldNotEqual(input);
        }

        [Fact]
        public void Should_decrypt_valid_data()
        {
            const string clear = "This is some input";
            var input = provider.Encrypt(clear);

            var output = provider.Decrypt(input);

            output.ShouldEqual(clear);
        }

        [Fact]
        public void Should_return_emptystring_for_invalid_data_when_decrypting()
        {
            const string clear = "This is some input";
            var input = provider.Encrypt(clear);
            input = input.Substring(0, input.Length - 3);

            var output = provider.Decrypt(input);

            output.ShouldEqual(string.Empty);
        }
    }
}
