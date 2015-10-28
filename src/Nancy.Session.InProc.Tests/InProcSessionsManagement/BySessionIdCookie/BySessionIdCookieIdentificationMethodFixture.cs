namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class BySessionIdCookieIdentificationMethodFixture
    {
        private readonly BySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;
        private readonly string cookieName;
        private readonly ICookieFactory fakeCookieFactory;
        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly IHmacProvider fakeHmacProvider;
        private readonly IHmacValidator fakeHmacValidator;
        private readonly ISessionIdentificationDataProvider fakeSessionIdentificationDataProvider;
        private readonly ISessionIdFactory fakeSessionIdFactory;
        private readonly InProcSessionsConfiguration validConfiguration;

        public BySessionIdCookieIdentificationMethodFixture()
        {
            this.fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.validConfiguration = new InProcSessionsConfiguration();
            this.fakeSessionIdentificationDataProvider = A.Fake<ISessionIdentificationDataProvider>();
            this.fakeHmacValidator = A.Fake<IHmacValidator>();
            this.fakeSessionIdFactory = A.Fake<ISessionIdFactory>();
            this.fakeCookieFactory = A.Fake<ICookieFactory>();
            this.bySessionIdCookieIdentificationMethod = new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory);
            this.cookieName = "TheNameOfTheCookie";
            this.bySessionIdCookieIdentificationMethod.CookieName = this.cookieName;
        }

        [Fact]
        public void Given_null_crypto_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(null));
        }

        [Fact]
        public void Given_null_encryption_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                null,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory));
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                null,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory));
        }

        [Fact]
        public void Given_null_session_data_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                null,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory));
        }

        [Fact]
        public void Given_null_hmac_validator_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                null,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory));
        }

        [Fact]
        public void Given_null_session_id_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                null,
                this.fakeCookieFactory));
        }

        [Fact]
        public void Given_null_cookie_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                null));
        }

        [Fact]
        public void On_creation_sets_default_cookie_name()
        {
            var newInstance = new BySessionIdCookieIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeCookieFactory);
            Assert.Equal("_nsid", newInstance.CookieName);
        }

        public class Load : BySessionIdCookieIdentificationMethodFixture
        {
            private readonly NancyContext context;
            private readonly SessionId newSessionId;

            public Load()
            {
                this.context = new NancyContext()
                {
                    Request = new Request("GET", "http://www.google.be")
                };

                this.newSessionId = new SessionId(Guid.NewGuid(), true);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .Returns(this.newSessionId);
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(null));
            }

            [Fact]
            public void When_context_contains_no_session_identification_data_then_returns_new_session_id()
            {
                A.CallTo(() => this.fakeSessionIdentificationDataProvider.ProvideDataFromCookie(
                    this.context.Request,
                    this.cookieName))
                    .Returns(null);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_cookie_does_not_have_a_valid_hmac_then_returns_new_session_id()
            {
                var cookieData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(() => this.fakeSessionIdentificationDataProvider.ProvideDataFromCookie(
                    this.context.Request,
                    this.cookieName))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(false);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_not_valid_then_returns_new_session_id()
            {
                var cookieData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(() => this.fakeSessionIdentificationDataProvider.ProvideDataFromCookie(
                    this.context.Request,
                    this.cookieName))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(cookieData.SessionId))
                    .Returns(string.Empty);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_not_a_valid_guid_then_returns_new_session_id()
            {
                const string invalidDecryptedSessionId = "This is not a valid guid!";
                var cookieData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(() => this.fakeSessionIdentificationDataProvider.ProvideDataFromCookie(
                    this.context.Request,
                    this.cookieName))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(cookieData.SessionId))
                    .Returns(invalidDecryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .Returns(null);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .MustHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_valid_then_returns_session_id_from_cookie()
            {
                var expectedSessionId = new SessionId(Guid.NewGuid(), false);
                var decryptedSessionId = expectedSessionId.Value.ToString();
                var cookieData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(() => this.fakeSessionIdentificationDataProvider.ProvideDataFromCookie(
                    this.context.Request,
                    this.cookieName))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(cookieData.SessionId))
                    .Returns(decryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .Returns(expectedSessionId);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(expectedSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .MustHaveHappened();
            }
        }

        public class Save : BySessionIdCookieIdentificationMethodFixture
        {
            private readonly NancyContext context;
            private readonly SessionId validSessionId;

            public Save()
            {
                this.validSessionId = new SessionId(Guid.NewGuid(), false);
                this.context = new NancyContext()
                {
                    Response = new Response()
                };
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.bySessionIdCookieIdentificationMethod.SaveSessionId(null, null));
            }

            [Fact]
            public void Given_null_session_id_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.bySessionIdCookieIdentificationMethod.SaveSessionId(null, this.context));
            }

            [Fact]
            public void Given_context_without_response_then_throws()
            {
                this.context.Response = null;
                Assert.Throws<ArgumentException>(
                    () => this.bySessionIdCookieIdentificationMethod.SaveSessionId(this.validSessionId, this.context));
            }

            [Fact]
            public void Given_empty_session_id_then_throws()
            {
                var emptySessionId = new SessionId(Guid.Empty, false);
                Assert.Throws<ArgumentException>(
                    () => this.bySessionIdCookieIdentificationMethod.SaveSessionId(emptySessionId, this.context));
            }

            [Fact]
            public void
                Adds_expected_cookie_to_response_containing_data_from_encryptionprovider_and_hmacprovider_and_returns_null
                ()
            {
                const string encryptedSessionId = "ABC_sessionid_xyz";
                var hmacBytes = new byte[] {1, 2, 3};
                var hmacString = Convert.ToBase64String(hmacBytes);
                var expectedCookieData = string.Format("{0}{1}", encryptedSessionId, hmacString);

                A.CallTo(() => this.fakeEncryptionProvider.Encrypt(this.validSessionId.Value.ToString()))
                    .Returns(encryptedSessionId);
                A.CallTo(() => this.fakeHmacProvider.GenerateHmac(encryptedSessionId))
                    .Returns(hmacBytes);
                A.CallTo(() => this.fakeHmacProvider.HmacLength)
                    .Returns(hmacBytes.Length);
                A.CallTo(() => this.fakeCookieFactory.CreateCookie(
                    this.cookieName,
                    null,
                    null,
                    A<SessionIdentificationData>.That.Matches(cookieData =>
                        cookieData.SessionId == encryptedSessionId &&
                        HmacComparer.Compare(cookieData.Hmac, hmacBytes, this.fakeHmacProvider.HmacLength))))
                    .Returns(new NancyCookie("cookiefortest", expectedCookieData));

                this.bySessionIdCookieIdentificationMethod.SaveSessionId(this.validSessionId, this.context);

                var addedCookie =
                    this.context.Response.Cookies.FirstOrDefault(cookie => cookie.Value == expectedCookieData);
                Assert.NotNull(addedCookie);
            }
        }
    }
}