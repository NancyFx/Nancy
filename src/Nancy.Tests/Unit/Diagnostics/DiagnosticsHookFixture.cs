namespace Nancy.Tests.Unit.Diagnostics
{
    using System;
    using System.Linq;

    using Nancy.Cookies;
    using Nancy.Diagnostics;
    using Nancy.Testing;

    using Xunit;

    public class DiagnosticsHookFixture
    {
        private const string DiagsCookieName = "__ncd";

        [Fact]
        public void Should_return_info_page_if_password_null()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = null };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy");

            result.Body.AsString().ShouldContain("Diagnostics Disabled");
        }

        [Fact]
        public void Should_return_info_page_if_password_empty()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = string.Empty };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy");

            result.Body.AsString().ShouldContain("Diagnostics Disabled");
        }

        [Fact]
        public void Should_return_login_page_with_no_auth_cookie()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy");

            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_main_page_with_valid_auth_cookie()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy", with =>
                {
                    with.Cookie(DiagsCookieName, this.GetAuthCookie("password"));
                });

            result.Body["#notSureWhatToPutHereYet"].ShouldExistOnce();
        }

        private string GetAuthCookie(string password, DateTime? expiry = null)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Should_return_login_page_with_expired_auth_cookie()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy", with =>
            {
                with.Cookie(DiagsCookieName, this.GetAuthCookie("password", DateTime.Now.AddMinutes(-10)));
            });

            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_login_page_with_auth_cookie_with_incorrect_password()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy", with =>
            {
                with.Cookie(DiagsCookieName, this.GetAuthCookie("wrongPassword"));
            });

            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_not_accept_invalid_password()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Post("/_Nancy", with =>
            {
                with.FormValue("Password", "wrongpassword");
            });

            result.Body["#error"].ShouldExistOnce();
            result.Cookies.Any(c => c.Name == DiagsCookieName).ShouldBeFalse();
        }

        [Fact]
        public void Should_set_login_cookie_when_password_correct()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Post("/_Nancy", with =>
            {
                with.FormValue("Password", "password");
            });

            result.Cookies.Any(c => c.Name == DiagsCookieName).ShouldBeTrue();
        }

        [Fact]
        public void Should_use_rolling_expiry_for_auth_cookie()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var expiryDate = DateTime.Now.AddMinutes(5);
            var result = browser.Get("/_Nancy", with =>
                {
                    with.Cookie(DiagsCookieName, this.GetAuthCookie("password", expiryDate));
                });

            result.Cookies.Any(c => c.Name == DiagsCookieName).ShouldBeTrue();
            this.DecodeCookie(result.Cookies.First(c => c.Name == DiagsCookieName))
                .Expiry.ShouldNotEqual(expiryDate);
        }

        private DiagnosticsSessionCookie DecodeCookie(INancyCookie nancyCookie)
        {
            throw new NotImplementedException();
        }
    }

    internal class DiagnosticsSessionCookie
    {
        public string Password { get; set; }

        public byte[] Salt { get; set; }

        public DateTime Expiry { get; set; }
    }
}