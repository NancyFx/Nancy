namespace Nancy.Tests.Unit.Security
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Security;
    using Xunit;

    public class DefaultCsrfTokenValidatorFixture
    {
        private readonly ICsrfTokenValidator validator;
        private readonly IHmacProvider hmacProvider;

        public DefaultCsrfTokenValidatorFixture()
        {
            var cryptoConfig = CryptographyConfiguration.Default;
            this.hmacProvider = cryptoConfig.HmacProvider;
            this.validator = new DefaultCsrfTokenValidator(cryptoConfig);            
        }

        [Fact]
        public void Should_return_token_missing_if_first_token_null()
        {
            var result = this.validator.Validate(null, new CsrfToken());

            result.ShouldEqual(CsrfTokenValidationResult.TokenMissing);
        }

        [Fact]
        public void Should_return_token_missing_if_second_token_null()
        {
            var result = this.validator.Validate(new CsrfToken(), null);

            result.ShouldEqual(CsrfTokenValidationResult.TokenMissing);
        }

        [Fact]
        public void Should_return_token_mismatch_if_tokens_differ()
        {
            DateTime date = DateTime.Now;
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 4, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo);

            result.ShouldEqual(CsrfTokenValidationResult.TokenMismatch);
        }

        [Fact]
        public void Should_return_token_ok_if_tokens_match_and_no_expiry_set()
        {
            DateTime date = DateTime.Now;
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo);

            result.ShouldEqual(CsrfTokenValidationResult.Ok);
        }

        [Fact]
        public void Should_return_token_mismatch_if_random_bytes_empty()
        {
            DateTime date = DateTime.Now;
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo);

            result.ShouldEqual(CsrfTokenValidationResult.TokenTamperedWith);
        }

        [Fact]
        public void Should_return_token_tampered_with_if_hmac_incorrect()
        {
            DateTime date = DateTime.Now;
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);
            tokenOne.Hmac[0] -= 1;
            tokenTwo.Hmac[0] -= 1;

            var result = this.validator.Validate(tokenOne, tokenTwo);

            result.ShouldEqual(CsrfTokenValidationResult.TokenTamperedWith);
        }

        [Fact]
        public void Should_return_token_expired_if_it_has()
        {
            DateTime date = DateTime.Now.AddHours(-1);
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo, validityPeriod: new TimeSpan(0, 30, 0));

            result.ShouldEqual(CsrfTokenValidationResult.TokenExpired);
        }

        [Fact]
        public void Should_return_ok_if_valid_and_not_expired()
        {
            DateTime date = DateTime.Now.AddHours(-1);
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo, validityPeriod: new TimeSpan(1, 30, 0));

            result.ShouldEqual(CsrfTokenValidationResult.Ok);
        }

        [Fact]
        public void Should_return_token_ok_if_tokens_match_and_no_salt()
        {
            DateTime date = DateTime.Now;
            var tokenOne = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            var tokenTwo = new CsrfToken { CreatedDate = date, RandomBytes = new byte[] { 1, 2, 3 } };
            tokenOne.CreateHmac(this.hmacProvider);
            tokenTwo.CreateHmac(this.hmacProvider);

            var result = this.validator.Validate(tokenOne, tokenTwo);

            result.ShouldEqual(CsrfTokenValidationResult.Ok);
        }
    }
}