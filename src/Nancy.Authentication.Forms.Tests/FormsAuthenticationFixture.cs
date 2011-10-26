namespace Nancy.Authentication.Forms.Tests
{
    using System;
    using System.Linq;
    using Bootstrapper;
    using Cryptography;
    using FakeItEasy;
    using Helpers;
    using Nancy.Security;
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
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000)),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000)));

            this.config = new FormsAuthenticationConfiguration()
            {
                CryptographyConfiguration = this.cryptographyConfiguration,
                RedirectUrl = "/login",
                UserMapper = A.Fake<IUserMapper>(),
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
            var result = Record.Exception(() => FormsAuthentication.Enable(A.Fake<IPipelines>(), null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_invalid_config_passed_to_enable()
        {
            var fakeConfig = A.Fake<FormsAuthenticationConfiguration>();
            A.CallTo(() => fakeConfig.IsValid).Returns(false);
            var result = Record.Exception(() => FormsAuthentication.Enable(A.Fake<IPipelines>(), fakeConfig));

            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_add_a_pre_and_post_hook_when_enabled()
        {
            var pipelines = A.Fake<IPipelines>();

            FormsAuthentication.Enable(pipelines, this.config);

            A.CallTo(() => pipelines.BeforeRequest.AddItemToStartOfPipeline(A<Func<NancyContext, Response>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => pipelines.AfterRequest.AddItemToEndOfPipeline(A<Action<NancyContext>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_redirect_response_when_user_logs_in_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }

        [Fact]
        public void Should_return_ok_response_when_user_logs_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid);

            // Then
            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_have_authentication_cookie_in_login_response_when_logging_in_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_have_authentication_cookie_in_login_response_when_logging_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid);

            // Then
            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_httponly_when_logging_in_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .HttpOnly.ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_httponly_when_logging_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid);

            // Then
            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .HttpOnly.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_set_expiry_date_if_one_not_specified_when_logging_in_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_expiry_date_if_one_not_specified_when_logging_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid);

            // Then
            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldBeNull();
        }

        [Fact]
        public void Should_set_expiry_date_if_one_specified_when_logging_in_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldNotBeNull();
        }

        [Fact]
        public void Should_set_expiry_date_if_one_specified_when_logging_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid, DateTime.Now.AddDays(1));

            // Then
            result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First()
                .Expires.ShouldNotBeNull();
        }

        [Fact]
        public void Should_encrypt_cookie_when_logging_in_with_redirect()
        {
            var mockEncrypter = A.Fake<IEncryptionProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(mockEncrypter, this.cryptographyConfiguration.HmacProvider);
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            A.CallTo(() => mockEncrypter.Encrypt(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_encrypt_cookie_when_logging_in_without_redirect()
        {
            // Given
            var mockEncrypter = A.Fake<IEncryptionProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(mockEncrypter, this.cryptographyConfiguration.HmacProvider);
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            FormsAuthentication.UserLoggedInResponse(userGuid, DateTime.Now.AddDays(1));

            // Then
            A.CallTo(() => mockEncrypter.Encrypt(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_generate_hmac_for_cookie_from_encrypted_cookie_when_logging_in_with_redirect()
        {
            var fakeEncrypter = A.Fake<IEncryptionProvider>();
            var fakeCryptoText = "FakeText";
            A.CallTo(() => fakeEncrypter.Encrypt(A<string>.Ignored))
                .Returns(fakeCryptoText);
            var mockHmac = A.Fake<IHmacProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(fakeEncrypter, mockHmac);
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, DateTime.Now.AddDays(1));

            A.CallTo(() => mockHmac.GenerateHmac(fakeCryptoText))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_generate_hmac_for_cookie_from_encrypted_cookie_when_logging_in_without_redirect()
        {
            // Given
            var fakeEncrypter = A.Fake<IEncryptionProvider>();
            var fakeCryptoText = "FakeText";
            A.CallTo(() => fakeEncrypter.Encrypt(A<string>.Ignored))
                .Returns(fakeCryptoText);
            var mockHmac = A.Fake<IHmacProvider>();
            this.config.CryptographyConfiguration = new CryptographyConfiguration(fakeEncrypter, mockHmac);
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            FormsAuthentication.UserLoggedInResponse(userGuid, DateTime.Now.AddDays(1));

            // Then
            A.CallTo(() => mockHmac.GenerateHmac(fakeCryptoText))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_redirect_response_when_user_logs_out_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.LogOutAndRedirectResponse(context, "/");

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }

        [Fact]
        public void Should_return_ok_response_when_user_logs_out_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.LogOutResponse();

            // Then
            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_have_expired_empty_authentication_cookie_in_logout_response_when_user_logs_out_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            var result = FormsAuthentication.LogOutAndRedirectResponse(context, "/");

            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Value.ShouldBeEmpty();
            cookie.Expires.ShouldNotBeNull();
            (cookie.Expires < DateTime.Now).ShouldBeTrue();
        }

        [Fact]
        public void Should_have_expired_empty_authentication_cookie_in_logout_response_when_user_logs_out_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.LogOutResponse();

            // Then
            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Value.ShouldBeEmpty();
            cookie.Expires.ShouldNotBeNull();
            (cookie.Expires < DateTime.Now).ShouldBeTrue();
        }

        [Fact]
        public void Should_get_username_from_mapping_service_with_valid_cookie()
        {
            var fakePipelines = new Pipelines();
            var mockMapper = A.Fake<IUserMapper>();
            this.config.UserMapper = mockMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.validCookieValue);

            fakePipelines.BeforeRequest.Invoke(this.context);

            A.CallTo(() => mockMapper.GetUserFromIdentifier(this.userGuid))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_user_in_context_with_valid_cookie()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = A.Fake<IUserIdentity>();
            fakeUser.UserName = "Bob";
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.validCookieValue);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.CurrentUser.ShouldBeSameAs(fakeUser);
        }

        [Fact]
        public void Should_not_set_user_in_context_with_invalid_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = A.Fake<IUserIdentity>();
            fakeUser.UserName = "Bob";
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithInvalidHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_user_in_context_with_empty_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = A.Fake<IUserIdentity>();
            fakeUser.UserName = "Bob";
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithEmptyHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_user_in_context_with_no_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = A.Fake<IUserIdentity>();
            fakeUser.UserName = "Bob";
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithNoHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_username_in_context_with_broken_encryption_data()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = A.Fake<IUserIdentity>();
            fakeUser.UserName = "Bob";
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithBrokenEncryptedData);

            var result = fakePipelines.BeforeRequest.Invoke(this.context);

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_retain_querystring_when_redirecting_to_login_page()
        {
            // Given
            var fakePipelines = new Pipelines();
            
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

            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            // When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(queryContext, userGuid, DateTime.Now.AddDays(1));

            // Then
            result.Headers["Location"].ShouldEqual("/secure?foo=bar");
        }
    }
}