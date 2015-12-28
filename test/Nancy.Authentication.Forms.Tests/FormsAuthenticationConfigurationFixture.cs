namespace Nancy.Authentication.Forms.Tests
{
    using System;
    using FakeItEasy;
    using Cryptography;
    using Nancy.Tests;
    using Xunit;

    public class FormsAuthenticationConfigurationFixture
    {
        private FormsAuthenticationConfiguration config;

        public FormsAuthenticationConfigurationFixture()
        {
            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));

            this.config = new FormsAuthenticationConfiguration()
                              {
                                  CryptographyConfiguration = cryptographyConfiguration,
                                  RedirectUrl = "/login",
                                  UserMapper = A.Fake<IUserMapper>(),
                              };
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set()
        {
            // Given, When, Then
            Assert.DoesNotThrow(() => config.EnsureConfigurationIsValid());
        }

        [Fact]
        public void Should_be_valid_with_empty_redirect_url_when_redirect_is_disabled()
        {
            // Given
            config.RedirectUrl = "";
            config.DisableRedirect = true;

            // When, Then
            Assert.DoesNotThrow(() => config.EnsureConfigurationIsValid());
        }

        [Fact]
        public void Should_not_be_valid_with_empty_redirect_url()
        {
            // Given
            config.RedirectUrl = "";

            // When
            var result = Record.Exception(() => config.EnsureConfigurationIsValid());

            // Then
            result.ShouldBeOfType<InvalidOperationException>();
            result.Message.ShouldEqual("When DisableRedirect is false RedirectUrl cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_with_null_username_mapper()
        {
            // Given
            config.UserMapper = null;

            // When
            var result = Record.Exception(() => config.EnsureConfigurationIsValid());

            // Then
            result.ShouldBeOfType<InvalidOperationException>();
            result.Message.ShouldEqual("UserMapper cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_with_null_cryptography_configuration()
        {
            // Given
            config.CryptographyConfiguration = null;

            // When
            var result = Record.Exception(() => config.EnsureConfigurationIsValid());

            // Then
            result.ShouldBeOfType<InvalidOperationException>();
            result.Message.ShouldEqual("CryptographyConfiguration cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_with_null_encryption_provider()
        {
            // Given
            config.CryptographyConfiguration = new CryptographyConfiguration(null, config.CryptographyConfiguration.HmacProvider);

            // When
            var result = Record.Exception(() => config.EnsureConfigurationIsValid());

            // Then
            result.ShouldBeOfType<InvalidOperationException>();
            result.Message.ShouldEqual("CryptographyConfiguration EncryptionProvider cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_with_null_hmac_provider()
        {
            // Given
            config.CryptographyConfiguration = new CryptographyConfiguration(config.CryptographyConfiguration.EncryptionProvider, null);

            // When
            var result = Record.Exception(() => config.EnsureConfigurationIsValid());

            // Then
            result.ShouldBeOfType<InvalidOperationException>();
            result.Message.ShouldEqual("CryptographyConfiguration HmacProvider cannot be null.");
        }
    }
}
