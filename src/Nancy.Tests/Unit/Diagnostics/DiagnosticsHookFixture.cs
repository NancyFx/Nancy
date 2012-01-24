namespace Nancy.Tests.Unit.Diagnostics
{
    using System;

    using Nancy.Testing;

    using Xunit;

    public class DiagnosticsHookFixture
    {
        [Fact]
        public void Should_return_info_page_if_password_null()
        {
            var bootstrapper = new ConfigurableBootstrapper();
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy");

            result.Body.AsString().ShouldContain("Diagnostics Disabled");
        }

        [Fact]
        public void Should_return_info_page_if_password_empty()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_return_login_page_with_no_auth_cookie()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_return_login_page_with_expired_auth_cookie()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_not_accept_invalid_password()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_set_login_cookie_when_password_correct()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_use_rolling_expiry_for_auth_cookie()
        {
            throw new NotImplementedException();
        }
    }
}