namespace Nancy.Authentication.Forms.Tests
{
    using FakeItEasy;
    using Nancy.Tests;
    using Xunit;

    public class FormsAuthenticationConfigurationFixture
    {
        private FormsAuthenticationConfiguration config;

        public FormsAuthenticationConfigurationFixture()
        {
            this.config = new FormsAuthenticationConfiguration()
                              {
                                  Passphrase = "SuperSecretPass",
                                  Salt = "AndVinegarCrisps",
                                  HmacPassphrase = "UberSuperSecure",
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
        public void Should_not_be_valid_with_empty_passphrase()
        {
            config.Passphrase = "";

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_salt_too_short()
        {
            config.Salt = "short";

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_empty_hmac_passphrase()
        {
            config.HmacPassphrase = "";

            var result = config.IsValid;

            result.ShouldBeFalse();
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
        public void Should_not_be_valid_with_null_encryption_provider()
        {
            config.EncryptionProvider = null;

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_hmac_provider()
        {
            config.HmacProvider = null;

            var result = config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_salt_bytes()
        {
            var result = config.SaltBytes;

            result.ShouldNotBeNull();
        }
    }
}