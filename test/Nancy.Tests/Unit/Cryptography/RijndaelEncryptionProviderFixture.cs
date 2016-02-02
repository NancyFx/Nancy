namespace Nancy.Tests.Unit.Cryptography
{
    using Nancy.Cryptography;

    using Xunit;

    public class RijndaelEncryptionProviderFixture
    {
        private RijndaelEncryptionProvider provider;

        public RijndaelEncryptionProviderFixture()
        {
            this.provider = new RijndaelEncryptionProvider(new PassphraseKeyGenerator("Passphrase", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
        }

        [Fact]
        public void Should_return_cypher_text_when_encrypting()
        {
            var inputText = "this is some text";

            var result = provider.Encrypt(inputText);
            
            result.ShouldNotEqual(inputText);
        }

        [Fact]
        public void Should_return_original_text_when_decrypting_with_same_key()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText);

            var result = provider.Decrypt(encText);

            result.ShouldEqual(inputText);
        }

        [Fact]
        public void Should_not_return_original_text_when_decrypting_with_different_keys()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText);

            var result = new RijndaelEncryptionProvider(new PassphraseKeyGenerator("Wrong", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })).Decrypt(encText);

            result.ShouldNotEqual(inputText);
        }
    }
}