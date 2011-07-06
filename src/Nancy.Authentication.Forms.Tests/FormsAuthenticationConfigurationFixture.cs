namespace Nancy.Authentication.Forms.Tests
{
    using FakeItEasy;

    using Nancy.Cryptography;
    using Nancy.Tests;
    using Xunit;

    public class FormsAuthenticationConfigurationFixture
    {
        private FormsAuthenticationConfiguration config;

        public FormsAuthenticationConfigurationFixture()
        {
            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass")), 
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure")));

            this.config = new FormsAuthenticationConfiguration()
                              {
                                  CryptographyConfiguration = cryptographyConfiguration,
                                  RedirectUrl = "/login",
                                  UsernameMapper = A.Fake<IUsernameMapper>(),
                              };
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set()
        {
            var result = config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_with_empty_redirect_url()
        {
            config.RedirectUrl = "";

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_username_mapper()
        {
            config.UsernameMapper = null;

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_cryptography_configuration()
        {
            config.CryptographyConfiguration = null;

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_encryption_provider()
        {
            config.CryptographyConfiguration = new CryptographyConfiguration(null, config.CryptographyConfiguration.HmacProvider);

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_hmac_provider()
        {
            config.CryptographyConfiguration = new CryptographyConfiguration(config.CryptographyConfiguration.EncryptionProvider, null); 

            var result = config.IsValid;

            result.ShouldBeFalse();
        }
    }
}