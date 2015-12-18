namespace Nancy.Tests.Unit.Cryptography
{
    using Nancy.Cryptography;

    using Xunit;

    public class NoEncryptionProviderFixture
    {
        private readonly IEncryptionProvider provider;

        public NoEncryptionProviderFixture()
        {
            this.provider = new NoEncryptionProvider();
        }

        [Fact]
        public void Should_return_cypher_text_when_encrypting()
        {
            var inputText = "this is some text";

            var result = provider.Encrypt(inputText);
            
            result.ShouldNotEqual(inputText);
        }

        [Fact]
        public void Should_return_original_text_when_decrypting()
        {
            var inputText = "this is some text";
            var encText = provider.Encrypt(inputText);

            var result = provider.Decrypt(encText);

            result.ShouldEqual(inputText);
        }
    }
}