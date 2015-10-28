namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam;
    using Xunit;

    public class ByQueryStringParamIdentificationMethodFixture
    {
        private readonly ByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod;
        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly IHmacProvider fakeHmacProvider;
        private readonly IHmacValidator fakeHmacValidator;
        private readonly IResponseManipulatorForSession fakeResponseManipulatorForSession;
        private readonly ISessionIdentificationDataProvider fakeSessionIdentificationDataProvider;
        private readonly ISessionIdFactory fakeSessionIdFactory;
        private readonly string parameterName;
        private readonly InProcSessionsConfiguration validConfiguration;

        public ByQueryStringParamIdentificationMethodFixture()
        {
            this.fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.validConfiguration = new InProcSessionsConfiguration();
            this.fakeSessionIdentificationDataProvider = A.Fake<ISessionIdentificationDataProvider>();
            this.fakeHmacValidator = A.Fake<IHmacValidator>();
            this.fakeSessionIdFactory = A.Fake<ISessionIdFactory>();
            this.fakeResponseManipulatorForSession = A.Fake<IResponseManipulatorForSession>();
            this.byQueryStringParamIdentificationMethod = new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession);
            this.parameterName = "TheNameOfTheParameter";
            this.byQueryStringParamIdentificationMethod.ParameterName = this.parameterName;
        }

        [Fact]
        public void Given_null_crypto_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(null));
        }

        [Fact]
        public void Given_null_encryption_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                null,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession));
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                null,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession));
        }

        [Fact]
        public void Given_null_session_data_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                null,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession));
        }

        [Fact]
        public void Given_null_hmac_validator_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                null,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession));
        }

        [Fact]
        public void Given_null_session_id_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                null,
                this.fakeResponseManipulatorForSession));
        }

        [Fact]
        public void Given_null_response_manipulator_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                null));
        }

        [Fact]
        public void On_creation_sets_default_parameter_name()
        {
            var newInstance = new ByQueryStringParamIdentificationMethod(
                this.fakeEncryptionProvider,
                this.fakeHmacProvider,
                this.fakeSessionIdentificationDataProvider,
                this.fakeHmacValidator,
                this.fakeSessionIdFactory,
                this.fakeResponseManipulatorForSession);
            Assert.Equal("_nsid", newInstance.ParameterName);
        }

        public class Load : ByQueryStringParamIdentificationMethodFixture
        {
            private readonly NancyContext context;
            private readonly SessionId newSessionId;

            public Load()
            {
                this.context = new NancyContext()
                {
                    Request = new Request("GET", "http://www.google.be?_nsid=01HMAC02SessionId")
                };

                this.newSessionId = new SessionId(Guid.NewGuid(), false);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .Returns(this.newSessionId);
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(null));
            }

            [Fact]
            public void When_context_contains_no_session_identification_data_then_returns_new_session_id()
            {
                A.CallTo(
                    () =>
                        this.fakeSessionIdentificationDataProvider.ProvideDataFromQuery(this.context.Request,
                            this.parameterName))
                    .Returns(null);

                var actual = this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_querystring_does_not_have_a_valid_hmac_then_returns_new_session_id()
            {
                var sessionIdentificationData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(
                    () =>
                        this.fakeSessionIdentificationDataProvider.ProvideDataFromQuery(this.context.Request,
                            this.parameterName))
                    .Returns(sessionIdentificationData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(sessionIdentificationData))
                    .Returns(false);

                var actual = this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_not_valid_then_returns_new_session_id()
            {
                var sessionIdentificationData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(
                    () =>
                        this.fakeSessionIdentificationDataProvider.ProvideDataFromQuery(this.context.Request,
                            this.parameterName))
                    .Returns(sessionIdentificationData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(sessionIdentificationData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(sessionIdentificationData.SessionId))
                    .Returns(string.Empty);

                var actual = this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(this.context);

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
                var sessionIdentificationData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(
                    () =>
                        this.fakeSessionIdentificationDataProvider.ProvideDataFromQuery(this.context.Request,
                            this.parameterName))
                    .Returns(sessionIdentificationData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(sessionIdentificationData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(sessionIdentificationData.SessionId))
                    .Returns(invalidDecryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .Returns(null);

                var actual = this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .MustHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_valid_then_returns_session_id_from_querystring()
            {
                var expectedSessionId = new SessionId(Guid.NewGuid(), false);
                var decryptedSessionId = expectedSessionId.Value.ToString();
                var sessionIdentificationData = new SessionIdentificationData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = new byte[] {1, 2, 3}
                };

                A.CallTo(
                    () =>
                        this.fakeSessionIdentificationDataProvider.ProvideDataFromQuery(this.context.Request,
                            this.parameterName))
                    .Returns(sessionIdentificationData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(sessionIdentificationData))
                    .Returns(true);
                A.CallTo(() => this.fakeEncryptionProvider.Decrypt(sessionIdentificationData.SessionId))
                    .Returns(decryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .Returns(expectedSessionId);

                var actual = this.byQueryStringParamIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(expectedSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .MustHaveHappened();
            }
        }

        public class Save : ByQueryStringParamIdentificationMethodFixture
        {
            private readonly NancyContext context;
            private readonly SessionId validSessionId;

            public Save()
            {
                this.validSessionId = new SessionId(Guid.NewGuid(), true);
                this.context = new NancyContext()
                {
                    Request = new Request("GET", "http://www.google.be")
                };
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.byQueryStringParamIdentificationMethod.SaveSessionId(this.validSessionId, null));
            }

            [Fact]
            public void Given_null_session_id_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.byQueryStringParamIdentificationMethod.SaveSessionId(null, this.context));
            }

            [Fact]
            public void Given_context_without_request_then_throws()
            {
                var contextWithoutRequest = new NancyContext();
                Assert.Throws<ArgumentException>(
                    () =>
                        this.byQueryStringParamIdentificationMethod.SaveSessionId(this.validSessionId,
                            contextWithoutRequest));
            }

            [Fact]
            public void Given_empty_session_id_then_throws()
            {
                var emptySessionId = new SessionId(Guid.Empty, false);
                Assert.Throws<ArgumentException>(
                    () => this.byQueryStringParamIdentificationMethod.SaveSessionId(emptySessionId, this.context));
            }

            [Fact]
            public void Given_session_id_is_not_new_then_does_nothing()
            {
                var existingSessionId = new SessionId(Guid.NewGuid(), false);
                this.byQueryStringParamIdentificationMethod.SaveSessionId(existingSessionId, this.context);
                A.CallTo(
                    () =>
                        this.fakeResponseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                            A<NancyContext>._,
                            A<SessionIdentificationData>._,
                            A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void Given_session_id_is_new_then_manipulates_response()
            {
                const string encryptedSessionId = "ABC_sessionid_xyz";
                var hmacBytes = new byte[] {1, 2, 3};

                A.CallTo(() => this.fakeEncryptionProvider.Encrypt(this.validSessionId.Value.ToString()))
                    .Returns(encryptedSessionId);
                A.CallTo(() => this.fakeHmacProvider.GenerateHmac(encryptedSessionId))
                    .Returns(hmacBytes);
                A.CallTo(() => this.fakeHmacProvider.HmacLength)
                    .Returns(hmacBytes.Length);

                this.byQueryStringParamIdentificationMethod.SaveSessionId(this.validSessionId, this.context);

                A.CallTo(() => this.fakeResponseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                    this.context,
                    A<SessionIdentificationData>.That.Matches(sid => sid.SessionId == encryptedSessionId &&
                                                                     HmacComparer.Compare(sid.Hmac, hmacBytes,
                                                                         this.fakeHmacProvider.HmacLength)),
                    this.parameterName))
                    .MustHaveHappened();
            }
        }
    }
}