namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Xunit;

    public class HmacValidatorFixture
    {
        private readonly IHmacProvider fakeHmacProvider;
        private readonly byte[] hmac;
        private readonly HmacValidator hmacValidator;
        private readonly SessionIdentificationData sessionIdentificationData;

        public HmacValidatorFixture()
        {
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.hmacValidator = new HmacValidator(this.fakeHmacProvider);
            this.hmac = new byte[] {1, 2, 3};
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "TheSessionId",
                Hmac = this.hmac
            };
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HmacValidator(null));
        }

        [Fact]
        public void Given_null_cookie_data_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.hmacValidator.IsValidHmac(null));
        }

        [Fact]
        public void Given_cookie_data_without_hmac_then_returns_false()
        {
            this.sessionIdentificationData.Hmac = null;
            var actual = this.hmacValidator.IsValidHmac(this.sessionIdentificationData);
            Assert.False(actual);
        }

        [Fact]
        public void Given_hmac_is_invalid_then_returns_false()
        {
            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(this.sessionIdentificationData.SessionId))
                .Returns(new byte[] {7, 8, 9, 10});
            A.CallTo(() => this.fakeHmacProvider.HmacLength)
                .Returns(4);
            var actual = this.hmacValidator.IsValidHmac(this.sessionIdentificationData);
            Assert.False(actual);
        }

        [Fact]
        public void Given_hmac_is_valid_then_returns_true()
        {
            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(this.sessionIdentificationData.SessionId))
                .Returns(this.hmac);
            A.CallTo(() => this.fakeHmacProvider.HmacLength)
                .Returns(this.hmac.Length);
            var actual = this.hmacValidator.IsValidHmac(this.sessionIdentificationData);
            Assert.True(actual);
        }
    }
}