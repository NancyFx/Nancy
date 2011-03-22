namespace Nancy.Tests.Unit.Sessions
{
    using Cryptography;
    using Session;
    using Xunit;

    public class DefaultEncryptionProviderFixture
    {
        private DefaultEncryptionProvider provider;
        private string passPhrase = "this is the passphrase";
        private byte[] salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public DefaultEncryptionProviderFixture()
        {
            this.provider = new DefaultEncryptionProvider();
        }

        [Fact]
        public void Should_return_cypher_text_when_encrypting()
        {
            var inputText = "this is some text";

            var result = provider.Encrypt(inputText, passPhrase, salt);
            
            result.ShouldNotEqual(inputText);
        }

        [Fact]
        public void Should_return_original_text_when_decrypting_with_same_pass_and_salt()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText, passPhrase, salt);

            var result = provider.Decrypt(encText, passPhrase, salt);

            result.ShouldEqual(inputText);
        }

        [Fact]
        public void Should_not_return_original_text_when_decrypting_with_different_pass()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText, passPhrase, salt);

            var result = provider.Decrypt(encText, "another passphrase", salt);

            result.ShouldNotEqual(inputText);
        }

        [Fact]
        public void Should_not_return_original_text_when_decrypting_with_different_salt()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText, passPhrase, salt);

            var result = provider.Decrypt(encText, passPhrase, new byte[] { 3, 2, 1, 4, 5, 6, 7, 8, 9, 0 });

            result.ShouldNotEqual(inputText);
        }
    }
}