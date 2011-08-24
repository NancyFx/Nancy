namespace Nancy.Authentication.Forms.Tests
{
    using System;
    using System.Linq;
    using Bootstrapper;
    using Cryptography;
    using FakeItEasy;
    using Helpers;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class FormsAuthenticationFixture
    {
        private FormsAuthenticationConfiguration config;
        private NancyContext context;
        private Guid userGuid;

        private string validCookieValue =
            HttpUtility.UrlEncode("C+QzBqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9");

        private string cookieWithNoHmac =
            HttpUtility.UrlEncode("k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9");

        private string cookieWithEmptyHmac =
            HttpUtility.UrlEncode("k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9");

        private string cookieWithInvalidHmac =
            HttpUtility.UrlEncode("C+QzbqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9");

        private string cookieWithBrokenEncryptedData =
            HttpUtility.UrlEncode("C+QzBqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3spwI0jB0KeVY9");

        private CryptographyConfiguration cryptographyConfiguration;

        public FormsAuthenticationFixture()
        {
            this.cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass")),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure")));

            this.config = new FormsAuthenticationConfiguration()
            {
                CryptographyConfiguration = this.cryptographyConfiguration,
                RedirectUrl = "/login",
                UsernameMapper = A.Fake<IUsernameMapper>(),
            };

            this.context = new NancyContext()
                               {
                                   Request = new FakeRequest("GET", "/")
                               };

            this.userGuid = new Guid("3D97EB33-824A-4173-A2C1-633AC16C1010");
        }

        [Fact]
        public void Should_throw_with_null_application_pipelines_passed_to_enable()
        {
            var result = Record.Exception(() => FormsAuthentication.Enable(null, this.config));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_null_config_passed_to_enable()
        {
            var result = Record.Exception(() => FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_invalid_config_passed_to_enable()
        {
            var fakeConfig = A.Fake<FormsAuthenticationConfiguration>();
            A.CallTo(() => fakeConfig.IsValid).Returns(false);
            var result = Record.Exception(() => FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), fakeConfig));

            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_add_a_pre_and_post_hook_when_enabled()
        {
            var pipelines = A.Fake<IApplicationPipelines>();

            FormsAuthentication.Enable(pipelines, this.config);

            A.CallTo(() => pipelines.BeforeRequest.AddItemToStartOfPipeline(A<Func<NancyContext, Response>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => pipelines.AfterRequest.AddItemToEndOfPipeline(A<Action<NancyContext>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_redirect_response_when_user_logs_in()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }

        [Fact]
        public void Should_have_authentication_cookie_in_login_response()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_httponly()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .HttpOnly.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_set_expiry_date_if_one_not_specified()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldBeNull();
        }

        [Fact]
        public void Should_set_expiry_date_if_one_specified()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldNotBeNull();
        }

        [Fact]
        public void Should_encrypt_cookie()
        {
            var mockEncrypter = A.Fake<IEncryptionProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(mockEncrypter, this.cryptographyConfiguration.HmacProvider);
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            A.CallTo(() => mockEncrypter.Encrypt(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_generate_hmac_for_cookie_from_encrypted_cookie()
        {
            var fakeEncrypter = A.Fake<IEncryptionProvider>();
            var fakeCryptoText = "FakeText";
            A.CallTo(() => fakeEncrypter.Encrypt(A<string>.Ignored))
                .Returns(fakeCryptoText);
            var mockHmac = A.Fake<IHmacProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(fakeEncrypter, mockHmac);
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            A.CallTo(() => mockHmac.GenerateHmac(fakeCryptoText))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_redirect_response_when_user_logs_out()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.LogOutAndRedirectResponse(context, "/");

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }

        [Fact]
        public void Should_have_expired_empty_authentication_cookie_in_logout_response()
        {
            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            var result = FormsAuthentication.LogOutAndRedirectResponse(context, "/");

            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Value.ShouldBeEmpty();
            cookie.Expires.ShouldNotBeNull();
            (cookie.Expires < DateTime.Now).ShouldBeTrue();
        }

        [Fact]
        public void Should_get_username_from_mapping_service_with_valid_cookie()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var mockMapper = A.Fake<IUsernameMapper>();
            this.config.UsernameMapper = mockMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.validCookieValue);

            fakePipelines.BeforeRequest.Invoke(this.context);

            A.CallTo(() => mockMapper.GetUsernameFromIdentifier(this.userGuid))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_username_in_context_with_valid_cookie()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var fakeMapper = A.Fake<IUsernameMapper>();
            A.CallTo(() => fakeMapper.GetUsernameFromIdentifier(this.userGuid)).Returns("Bob");
            this.config.UsernameMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.validCookieValue);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.Items[Security.SecurityConventions.AuthenticatedUsernameKey].ShouldEqual("Bob");
        }

        [Fact]
        public void Should_not_set_username_in_context_with_invalid_hmac()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var fakeMapper = A.Fake<IUsernameMapper>();
            A.CallTo(() => fakeMapper.GetUsernameFromIdentifier(this.userGuid)).Returns("Bob");
            this.config.UsernameMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithInvalidHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.Items.ContainsKey(Security.SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Should_not_set_username_in_context_with_empty_hmac()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var fakeMapper = A.Fake<IUsernameMapper>();
            A.CallTo(() => fakeMapper.GetUsernameFromIdentifier(this.userGuid)).Returns("Bob");
            this.config.UsernameMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithEmptyHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.Items.ContainsKey(Security.SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Should_not_set_username_in_context_with_no_hmac()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var fakeMapper = A.Fake<IUsernameMapper>();
            A.CallTo(() => fakeMapper.GetUsernameFromIdentifier(this.userGuid)).Returns("Bob");
            this.config.UsernameMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithNoHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.Items.ContainsKey(Security.SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Should_not_set_username_in_context_with_broken_encryption_data()
        {
            var fakePipelines = new FakeApplicationPipelines();
            var fakeMapper = A.Fake<IUsernameMapper>();
            A.CallTo(() => fakeMapper.GetUsernameFromIdentifier(this.userGuid)).Returns("Bob");
            this.config.UsernameMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithBrokenEncryptedData);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.Items.ContainsKey(Security.SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Should_retain_querystring_when_redirecting_to_login_page()
        {
            // Given
            var fakePipelines = new FakeApplicationPipelines();
            
            FormsAuthentication.Enable(fakePipelines, this.config);

            var queryContext = new NancyContext()
            {
                Request = new FakeRequest("GET", "/secure", "?foo=bar"),
                Response = HttpStatusCode.Unauthorized
            };

            // When
            fakePipelines.AfterRequest.Invoke(queryContext);

            // Then
            queryContext.Response.Headers["Location"].ShouldEqual("/login?returnUrl=/secure%3ffoo%3dbar");
        }

        [Fact]
        public void Should_retain_querystring_when_redirecting_after_successfull_login()
        {
            // Given
            var queryContext = new NancyContext()
            {
                Request = new FakeRequest("GET", "/secure", "returnUrl=/secure%3Ffoo%3Dbar")
            };

            FormsAuthentication.Enable(A.Fake<IApplicationPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(queryContext, userGuid, DateTime.Now.AddDays(1));

            // Then
            result.Headers["Location"].ShouldEqual("/secure?foo=bar");
        }

        public class FakeApplicationPipelines : IApplicationPipelines
        {
            public BeforePipeline BeforeRequest { get; set; }

            public AfterPipeline AfterRequest { get; set; }

            public FakeApplicationPipelines()
            {
                this.BeforeRequest = new BeforePipeline();
                this.AfterRequest = new AfterPipeline();
            }
        }
    }
}