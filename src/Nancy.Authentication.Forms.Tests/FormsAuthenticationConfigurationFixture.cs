namespace Nancy.Authentication.Forms.Tests
{
    using FakeItEasy;

    using Nancy.Cryptography;
    using Nancy.Tests;
    using Xunit;

    public class FormsAuthenticationConfigurationFixture
    {
        private FormsAuthenticationConfiguration config;
        private FormsAuthenticationConfiguration asyncConfig;

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
            this.asyncConfig = new FormsAuthenticationConfiguration()
                              {
                                  CryptographyConfiguration = cryptographyConfiguration,
                                  RedirectUrl = "/login",
                                  AsyncUserMapper = A.Fake<IAsyncUserMapper>(),
                              };
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set_and_a_non_async_user_mapper()
        {
            var result = config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set_and_an_async_user_mapper()
        {
            var result = this.asyncConfig.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_valid_with_empty_redirect_url_when_redirect_is_disabled()
        {
            config.RedirectUrl = "";
            config.DisableRedirect = true;

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
        public void Should_not_be_valid_with_null_username_mapper_and_null_async_username_mapper()
        {
            config.UserMapper = null;

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_non_null_username_mapper_and_non_null_async_username_mapper()
        {
            config.AsyncUserMapper = this.asyncConfig.AsyncUserMapper;

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