namespace Nancy.Testing.Tests
{
    using FakeItEasy;

    using Nancy.Cryptography;
    using Nancy.Session;
    using Nancy.Tests;

    using Xunit;

    public class CookieBasesSessionsConfigurationFixture
    {
        private readonly CookieBasedSessionsConfiguration config;

        public CookieBasesSessionsConfigurationFixture()
        {
            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));

            this.config = new CookieBasedSessionsConfiguration()
            {
                CryptographyConfiguration = cryptographyConfiguration,
                Serializer = A.Fake<IObjectSerializer>()
            };
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set()
        {
            var result = this.config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_with_empty_cookiename()
        {
            this.config.CookieName = "";

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_serializer()
        {
            this.config.Serializer = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_cryptography_configuration()
        {
            this.config.CryptographyConfiguration = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_encryption_provider()
        {
            this.config.CryptographyConfiguration = new CryptographyConfiguration(null, this.config.CryptographyConfiguration.HmacProvider);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_hmac_provider()
        {
            this.config.CryptographyConfiguration = new CryptographyConfiguration(this.config.CryptographyConfiguration.EncryptionProvider, null);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }
    }
}