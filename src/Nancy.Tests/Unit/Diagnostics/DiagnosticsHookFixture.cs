namespace Nancy.Tests.Unit.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Diagnostics;
    using Nancy.Testing;

    using Xunit;

    public class DiagnosticsHookFixture
    {
        private const string DiagsCookieName = "__ncd";

        private readonly CryptographyConfiguration cryptoConfig;

        private readonly IObjectSerializer objectSerializer;

        public DiagnosticsHookFixture()
        {
            this.cryptoConfig = CryptographyConfiguration.Default;
            this.objectSerializer = new DefaultObjectSerializer();
        }

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
                    with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password"));
                });

            result.Body["#notSureWhatToPutHereYet"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_login_page_with_expired_auth_cookie()
        {
            var diagsConfig = new DiagnosticsConfiguration { Password = "password" };
            var bootstrapper = new ConfigurableBootstrapper(b => b.DiagnosticsConfiguration(diagsConfig));
            var browser = new Browser(bootstrapper);

            var result = browser.Get("/_Nancy", with =>
            {
                with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password", DateTime.Now.AddMinutes(-10)));
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
                with.Cookie(DiagsCookieName, this.GetSessionCookieValue("wrongPassword"));
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
                    with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password", expiryDate));
                });

            result.Cookies.Any(c => c.Name == DiagsCookieName).ShouldBeTrue();
            this.DecodeCookie(result.Cookies.First(c => c.Name == DiagsCookieName))
                .Expiry.ShouldNotEqual(expiryDate);
        }

        private string GetSessionCookieValue(string password, DateTime? expiry = null)
        {
            var salt = DiagnosticsSession.GenerateRandomSalt();
            var hash = DiagnosticsSession.GenerateSaltedHash(password, salt);
            var session = new DiagnosticsSession
                {
                    Hash = hash,
                    Salt = salt,
                    Expiry = expiry.HasValue ? expiry.Value : DateTime.Now.AddMinutes(15),
                };

            var serializedSession = this.objectSerializer.Serialize(session);
        }

        private DiagnosticsSession DecodeCookie(INancyCookie nancyCookie)
        {
            throw new NotImplementedException();
        }
    }

    internal class DiagnosticsSession
    {
        public byte[] Hash { get; set; }

        public byte[] Salt { get; set; }

        public DateTime Expiry { get; set; }

        public static byte[] GenerateRandomSalt()
        {
            var provider = new RNGCryptoServiceProvider();

            var buffer = new byte[32];
            provider.GetBytes(buffer);

            return buffer;
        }

        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            return GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), salt);
        }
    }
}