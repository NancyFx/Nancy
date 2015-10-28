namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class CookieFactoryFixture
    {
        private readonly string cookieDomain;
        private readonly CookieFactory cookieFactory;
        private readonly string cookieName;
        private readonly string cookiePath;
        private readonly string cookieValue;
        private readonly string cookieValueEncoded;
        private readonly SessionIdentificationData sessionIdentificationData;

        public CookieFactoryFixture()
        {
            this.cookieFactory = new CookieFactory();

            this.cookieName = "TheCookieName";
            this.cookieValue = "01HMAC98%02SessionId";
            this.cookieValueEncoded = "01HMAC98%2502SessionId";
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "%02SessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.cookieDomain = ".nascar.com";
            this.cookiePath = "/schedule/";
        }

        [Fact]
        public void Given_null_cookie_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.cookieFactory.CreateCookie(
                null,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData));
        }

        [Fact]
        public void Given_empty_cookie_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.cookieFactory.CreateCookie(
                string.Empty,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData));
        }

        [Fact]
        public void Given_whitespace_cookie_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.cookieFactory.CreateCookie(
                " ",
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData));
        }

        [Fact]
        public void Given_null_cookie_domain_then_does_not_throw()
        {
            Assert.DoesNotThrow(() => this.cookieFactory.CreateCookie(
                this.cookieName,
                null,
                this.cookiePath,
                this.sessionIdentificationData));
        }

        [Fact]
        public void Given_null_cookie_path_then_does_not_throw()
        {
            Assert.DoesNotThrow(() => this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                null,
                this.sessionIdentificationData));
        }

        [Fact]
        public void Returns_http_only_cookie()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData);
            Assert.True(actualCookie.HttpOnly);
        }

        [Fact]
        public void Returns_cookie_without_path_and_domain_if_none_is_set()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                null,
                null,
                this.sessionIdentificationData);
            Assert.Null(actualCookie.Domain);
            Assert.Null(actualCookie.Path);
        }

        [Fact]
        public void Returns_cookie_with_path_and_domain_if_specified()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData);
            Assert.Equal(this.cookieDomain, actualCookie.Domain);
            Assert.Equal(this.cookiePath, actualCookie.Path);
        }

        [Fact]
        public void Returns_cookie_with_valid_name()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData);
            Assert.Equal(this.cookieName, actualCookie.Name);
        }

        [Fact]
        public void Returns_cookie_with_specified_data()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData);
            Assert.Equal(this.cookieValue, actualCookie.Value);
        }

        [Fact]
        public void Returns_cookie_with_specified_encoded_data()
        {
            var actualCookie = this.cookieFactory.CreateCookie(
                this.cookieName,
                this.cookieDomain,
                this.cookiePath,
                this.sessionIdentificationData);
            Assert.Equal(this.cookieValueEncoded, actualCookie.EncodedValue);
        }
    }
}