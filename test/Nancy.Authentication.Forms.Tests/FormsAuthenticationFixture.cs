namespace Nancy.Authentication.Forms.Tests
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using FakeItEasy;
    using Bootstrapper;
    using Cryptography;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class FormsAuthenticationFixture
    {
        private FormsAuthenticationConfiguration config;
        private FormsAuthenticationConfiguration secureConfig;
        private FormsAuthenticationConfiguration domainPathConfig;
        private NancyContext context;
        private Guid userGuid;

        private string validCookieValue =
            "C+QzBqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9";

        private string cookieWithNoHmac =
            "k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9";

        private string cookieWithEmptyHmac =
            "k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9";

        private string cookieWithInvalidHmac =
            "C+QzbqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3sZwI0jB0KeVY9";

        private string cookieWithBrokenEncryptedData =
            "C+QzBqI2qSE6Qk60fmCsoMsQNLbQtCAFd5cpcy1xhu4=k+1IvvzkgKgfOK2/EgIr7Ut15f47a0fnlgH9W+Lzjv/a2Zkfxg3spwI0jB0KeVY9";

        private CryptographyConfiguration cryptographyConfiguration;

        private string domain = ".nancyfx.org";

        private string path = "/";

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
                RequiresSSL = false
            };

            this.secureConfig = new FormsAuthenticationConfiguration()
            {
                CryptographyConfiguration = this.cryptographyConfiguration,
                RedirectUrl = "/login",
                UserMapper = A.Fake<IUserMapper>(),
                RequiresSSL = true
            };

            this.domainPathConfig = new FormsAuthenticationConfiguration()
            {
                CryptographyConfiguration = this.cryptographyConfiguration,
                RedirectUrl = "/login",
                UserMapper = A.Fake<IUserMapper>(),
                RequiresSSL = false,
                Domain = domain,
                Path = path
            };

            this.context = new NancyContext
                               {
                                    Request = new Request(
                                                    "GET",
                                                    new Url { Scheme = "http", BasePath = "/testing", HostName = "test.com", Path = "test" })
                               };

            this.userGuid = new Guid("3D97EB33-824A-4173-A2C1-633AC16C1010");
        }

        [Fact]
        public void Should_throw_with_null_application_pipelines_passed_to_enable()
        {
            var result = Record.Exception(() => FormsAuthentication.Enable((IPipelines)null, this.config));

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
            A.CallTo(() => fakeConfig.EnsureConfigurationIsValid()).Throws<InvalidOperationException>();
            var result = Record.Exception(() => FormsAuthentication.Enable(A.Fake<IPipelines>(), fakeConfig));

            result.ShouldBeOfType(typeof(InvalidOperationException));
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
        public void Should_add_a_pre_hook_but_not_a_post_hook_when_DisableRedirect_is_true()
        {
            var pipelines = A.Fake<IPipelines>();

            this.config.DisableRedirect = true;
            FormsAuthentication.Enable(pipelines, this.config);

            A.CallTo(() => pipelines.BeforeRequest.AddItemToStartOfPipeline(A<Func<NancyContext, Response>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => pipelines.AfterRequest.AddItemToEndOfPipeline(A<Action<NancyContext>>.Ignored))
                .MustNotHaveHappened();
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
            //Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);

            //When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            //Then
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
            var cookie = result.Cookies.First(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName);
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

            fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            A.CallTo(() => mockMapper.GetUserFromIdentifier(this.userGuid, this.context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_user_in_context_with_valid_cookie()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.validCookieValue);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            context.CurrentUser.ShouldBeSameAs(fakeUser);
        }

        [Fact]
        public void Should_not_set_user_in_context_with_empty_cookie()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, string.Empty);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_user_in_context_with_invalid_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithInvalidHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_user_in_context_with_empty_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithEmptyHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_user_in_context_with_no_hmac()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithNoHmac);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Should_not_set_username_in_context_with_broken_encryption_data()
        {
            var fakePipelines = new Pipelines();
            var fakeMapper = A.Fake<IUserMapper>();
            var fakeUser = new ClaimsPrincipal(new GenericIdentity("Bob"));
            A.CallTo(() => fakeMapper.GetUserFromIdentifier(this.userGuid, this.context)).Returns(fakeUser);
            this.config.UserMapper = fakeMapper;
            FormsAuthentication.Enable(fakePipelines, this.config);
            this.context.Request.Cookies.Add(FormsAuthentication.FormsAuthenticationCookieName, this.cookieWithBrokenEncryptedData);

            var result = fakePipelines.BeforeRequest.Invoke(this.context, new CancellationToken());

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
            fakePipelines.AfterRequest.Invoke(queryContext, new CancellationToken());

            // Then
            queryContext.Response.Headers["Location"].ShouldEqual("/login?returnUrl=/secure%3ffoo%3dbar");
        }

        [Fact]
        public void Should_change_the_forms_authentication_redirect_uri_querystring_key()
        {
            // Given
            var fakePipelines = new Pipelines();

            this.config.RedirectQuerystringKey = "next";
            FormsAuthentication.Enable(fakePipelines, this.config);

            var queryContext = new NancyContext()
            {
                Request = new FakeRequest("GET", "/secure", "?foo=bar"),
                Response = HttpStatusCode.Unauthorized
            };

            // When
            fakePipelines.AfterRequest.Invoke(queryContext, new CancellationToken());

            // Then
            queryContext.Response.Headers["Location"].ShouldEqual("/login?next=/secure%3ffoo%3dbar");
        }

        [Fact]
        public void Should_change_the_forms_authentication_redirect_uri_querystring_key_returnUrl_if_config_redirectQuerystringKey_is_null()
        {
            // Given
            var fakePipelines = new Pipelines();

            this.config.RedirectQuerystringKey = null;
            FormsAuthentication.Enable(fakePipelines, this.config);

            var queryContext = new NancyContext()
            {
                Request = new FakeRequest("GET", "/secure", "?foo=bar"),
                Response = HttpStatusCode.Unauthorized
            };

            // When
            fakePipelines.AfterRequest.Invoke(queryContext, new CancellationToken());

            // Then
            queryContext.Response.Headers["Location"].ShouldEqual("/login?returnUrl=/secure%3ffoo%3dbar");
        }

        [Fact]
        public void Should_change_the_forms_authentication_redirect_uri_querystring_key_returnUrl_if_config_redirectQuerystringKey_is_empty()
        {
            // Given
            var fakePipelines = new Pipelines();

            this.config.RedirectQuerystringKey = string.Empty;
            FormsAuthentication.Enable(fakePipelines, this.config);

            var queryContext = new NancyContext()
            {
                Request = new FakeRequest("GET", "/secure", "?foo=bar"),
                Response = HttpStatusCode.Unauthorized
            };

            // When
            fakePipelines.AfterRequest.Invoke(queryContext, new CancellationToken());

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

        [Fact]
        public void Should_set_authentication_cookie_to_secure_when_config_requires_ssl_and_logging_in_with_redirect()
        {
            //Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.secureConfig);

            //When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            //Then
            result.Cookies
                    .Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName)
                    .First()
                    .Secure.ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_secure_when_config_requires_ssl_and_logging_in_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.secureConfig);

            // When
            var result = FormsAuthentication.UserLoggedInResponse(userGuid);

            // Then
            result.Cookies
                    .Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName)
                    .First()
                    .Secure.ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_secure_when_config_requires_ssl_and_user_logs_out_with_redirect()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.secureConfig);

            var result = FormsAuthentication.LogOutAndRedirectResponse(context, "/");

            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Secure.ShouldBeTrue();
        }

        [Fact]
        public void Should_set_authentication_cookie_to_secure_when_config_requires_ssl_and_user_logs_out_without_redirect()
        {
            // Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.secureConfig);

            // When
            var result = FormsAuthentication.LogOutResponse();

            // Then
            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Secure.ShouldBeTrue();
        }

        [Fact]
        public void Should_redirect_to_base_path_if_non_local_url_and_no_fallback()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);
            context.Request.Query[config.RedirectQuerystringKey] = "http://moo.com/";

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
            result.Headers["Location"].ShouldEqual("/testing");
        }

        [Fact]
        public void Should_redirect_to_fallback_if_non_local_url_and_fallback_set()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);
            context.Request.Query[config.RedirectQuerystringKey] = "http://moo.com/";

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid, fallbackRedirectUrl:"/moo");

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
            result.Headers["Location"].ShouldEqual("/moo");
        }

        [Fact]
        public void Should_redirect_to_given_url_if_local()
        {
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.config);
            context.Request.Query[config.RedirectQuerystringKey] = "~/login";

            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            result.ShouldBeOfType(typeof(Response));
            result.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
            result.Headers["Location"].ShouldEqual("/testing/login");
        }

        [Fact]
        public void Should_set_Domain_when_config_provides_domain_value()
        {
            //Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.domainPathConfig);

            //When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            //Then
            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Domain.ShouldEqual(domain);
        }

        [Fact]
        public void Should_set_Path_when_config_provides_path_value()
        {
            //Given
            FormsAuthentication.Enable(A.Fake<IPipelines>(), this.domainPathConfig);

            //When
            var result = FormsAuthentication.UserLoggedInRedirectResponse(context, userGuid);

            //Then
            var cookie = result.Cookies.Where(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName).First();
            cookie.Path.ShouldEqual(path);
        }

        [Fact]
        public void Should_throw_with_null_module_passed_to_enable()
        {
            var result = Record.Exception(() => FormsAuthentication.Enable((INancyModule)null, this.config));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_null_config_passed_to_enable_with_module()
        {
            var result = Record.Exception(() => FormsAuthentication.Enable(new FakeModule(), null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        class FakeModule : NancyModule
        {
            public FakeModule()
            {
                this.After = new AfterPipeline();
                this.Before = new BeforePipeline();
                this.OnError = new ErrorPipeline();
            }
        }
    }
}
