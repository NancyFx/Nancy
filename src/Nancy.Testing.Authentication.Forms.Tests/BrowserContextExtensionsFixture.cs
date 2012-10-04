namespace Nancy.Testing.Authentication.Forms.Tests
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Helpers;
    using Nancy.Authentication.Forms;
    using Nancy.Tests;
    using Session;
    using Xunit;

    public class BrowserContextExtensionsFixture
    {
        private readonly Browser browser;

        public BrowserContextExtensionsFixture()
        {
            var bootstrapper =
                new ConfigurableBootstrapper(config => config.Modules(typeof(ModuleUnderTest)));

            CookieBasedSessions.Enable(bootstrapper);

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_add_forms_authentication_cookie_to_the_request()
        {
            // Given
            var userId = A.Dummy<Guid>();

            var formsAuthConfig = 
                new FormsAuthenticationConfiguration
                {
                    RedirectUrl = "/login",
                    UserMapper = A.Fake<IUserMapper>(),
                };

            var encryptedId = 
                formsAuthConfig.CryptographyConfiguration.EncryptionProvider.Encrypt(userId.ToString());
            
            var hmacBytes = 
                formsAuthConfig.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedId);

            var hmacString = 
                Convert.ToBase64String(hmacBytes);
            
            var cookieContents = 
                String.Format("{1}{0}", encryptedId, hmacString);

            // When
            var response = this.browser.Get("/cookie", with =>
            {
                with.HttpRequest();
                with.FormsAuth(userId, formsAuthConfig);
            });

            var cookie = response.Cookies.Single(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName);
            var cookieValue = HttpUtility.UrlDecode(cookie.Value);

            // Then
            cookieValue.ShouldEqual(cookieContents);
        }

        public class ModuleUnderTest : NancyModule
        {
            public ModuleUnderTest()
            {
                Get["/cookie"] = ctx =>
                {
                    var response = (Response)"Cookies";

                    foreach (var cookie in Request.Cookies)
                    {
                        response.AddCookie(cookie.Key, cookie.Value);
                    }

                    return response;
                };
            }
        }
    }
}
