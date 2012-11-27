namespace Nancy.Tests.Unit.Diagnostics
{
    using System;
    using System.Linq;
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

#if DEBUG
        [Fact]
        public void Should_return_info_page_if_password_null()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = null, CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>{
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path);

            // Then
            Assert.True(result.Body.AsString().Contains("Diagnostics Disabled"));
        }

        [Fact]
        public void Should_return_info_page_if_password_empty()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = string.Empty, CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path);

            // Then
            Assert.True(result.Body.AsString().Contains("Diagnostics Disabled"));
        }
#endif

        [Fact]
        public void Should_return_login_page_with_no_auth_cookie()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path);

            // Then
            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_main_page_with_valid_auth_cookie()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path, with =>
                {
                    with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password"));
                });

            // Then
            result.Body["#infoBox"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_login_page_with_expired_auth_cookie()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path, with =>
            {
                with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password", DateTime.Now.AddMinutes(-10)));
            });

            // Then
            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_return_login_page_with_auth_cookie_with_incorrect_password()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(diagsConfig.Path, with =>
            {
                with.Cookie(DiagsCookieName, this.GetSessionCookieValue("wrongPassword"));
            });

            // Then
            result.Body["#login"].ShouldExistOnce();
        }

        [Fact]
        public void Should_not_accept_invalid_password()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Post(diagsConfig.Path, with =>
            {
                with.FormValue("Password", "wrongpassword");
            });

            // Then
            result.Body["#login"].ShouldExistOnce();
            result.Cookies.Any(c => c.Name == DiagsCookieName && !string.IsNullOrEmpty(c.Value)).ShouldBeFalse();
        }

        [Fact]
        public void Should_set_login_cookie_when_password_correct()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Post(diagsConfig.Path, with =>
            {
                with.FormValue("Password", "password");
            });

            // Then
            result.Cookies.Any(c => c.Name == DiagsCookieName).ShouldBeTrue();
            string.IsNullOrEmpty(result.Cookies.First(c => c.Name == DiagsCookieName).Value).ShouldBeFalse();
        }

        [Fact]
        public void Should_use_rolling_expiry_for_auth_cookie()
        {
            // Given
            var diagsConfig = new DiagnosticsConfiguration { Password = "password", CryptographyConfiguration = this.cryptoConfig };

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.EnableAutoRegistration();
                with.DiagnosticsConfiguration(diagsConfig);
                with.Diagnostics<DefaultDiagnostics>();
            });

            var browser = new Browser(bootstrapper);
            var expiryDate = DateTime.Now.AddMinutes(5);

            // When
            var result = browser.Get(diagsConfig.Path, with =>
            {
                with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password", expiryDate));
            });

            // Then
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

            var encryptedSession = this.cryptoConfig.EncryptionProvider.Encrypt(serializedSession);
            var hmacBytes = this.cryptoConfig.HmacProvider.GenerateHmac(encryptedSession);
            var hmacString = Convert.ToBase64String(hmacBytes);

            return String.Format("{1}{0}", encryptedSession, hmacString);
        }

        private DiagnosticsSession DecodeCookie(INancyCookie nancyCookie)
        {
            var cookieValue = nancyCookie.Value;
            var hmacStringLength = Base64Helpers.GetBase64Length(this.cryptoConfig.HmacProvider.HmacLength);
            var encryptedSession = cookieValue.Substring(hmacStringLength);
            var decrypted = this.cryptoConfig.EncryptionProvider.Decrypt(encryptedSession);
            
            return this.objectSerializer.Deserialize(decrypted) as DiagnosticsSession;
        }
    }
}
