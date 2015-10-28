namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using FakeItEasy;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Xunit;

    public class InProcSessionManagerFixture
    {
        private readonly IPeriodicCacheCleaner fakePeriodicCacheCleaner;
        private readonly IInProcSessionCache fakeSessionCache;
        private readonly IInProcSessionFactory fakeSessionFactory;
        private readonly IInProcSessionIdentificationMethod fakeSessionIdentificationMethod;
        private readonly NancyContext nancyContext;
        private readonly InProcSessionManager sessionManager;
        private readonly InProcSessionsConfiguration validConfiguration;

        public InProcSessionManagerFixture()
        {
            this.nancyContext = new NancyContext();
            this.fakeSessionIdentificationMethod = A.Fake<IInProcSessionIdentificationMethod>();
            this.validConfiguration = new InProcSessionsConfiguration
            {
                SessionIdentificationMethod = this.fakeSessionIdentificationMethod,
                SessionTimeout = TimeSpan.FromMinutes(30),
                CacheTrimInterval = TimeSpan.FromMinutes(40)
            };
            this.fakeSessionCache = A.Fake<IInProcSessionCache>();
            this.fakeSessionFactory = A.Fake<IInProcSessionFactory>();
            this.fakePeriodicCacheCleaner = A.Fake<IPeriodicCacheCleaner>();
            this.sessionManager = new InProcSessionManager(
                this.validConfiguration,
                this.fakeSessionCache,
                this.fakeSessionFactory,
                this.fakePeriodicCacheCleaner);
        }

        [Fact]
        public void Given_null_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionManager(
                null,
                this.fakeSessionCache,
                this.fakeSessionFactory,
                this.fakePeriodicCacheCleaner));
        }

        [Fact]
        public void Given_null_cache_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionManager(
                this.validConfiguration,
                null,
                this.fakeSessionFactory,
                this.fakePeriodicCacheCleaner));
        }

        [Fact]
        public void Given_null_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionManager(
                this.validConfiguration,
                this.fakeSessionCache,
                null,
                this.fakePeriodicCacheCleaner));
        }

        [Fact]
        public void Given_null_periodic_cleaner_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionManager(
                this.validConfiguration,
                this.fakeSessionCache,
                this.fakeSessionFactory,
                null));
        }

        [Fact]
        public void When_constructing_then_starts_periodic_clean_task()
        {
            A.CallTo(() => this.fakePeriodicCacheCleaner.Start())
                .MustHaveHappened();
        }

        public class Load : InProcSessionManagerFixture
        {
            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(() => this.sessionManager.Load(null));
            }

            [Fact]
            public void Loads_session_with_id_from_identification_method()
            {
                var sessionId = new SessionId(Guid.NewGuid(), false);
                var expectedSession = new InProcSession(sessionId, A.Fake<ISession>(), DateTime.Now,
                    TimeSpan.FromMinutes(10));

                A.CallTo(() => this.fakeSessionIdentificationMethod.GetCurrentSessionId(this.nancyContext))
                    .Returns(sessionId);
                A.CallTo(() => this.fakeSessionCache.Get(sessionId))
                    .Returns(expectedSession);

                var actual = this.sessionManager.Load(this.nancyContext);

                Assert.Equal(expectedSession, actual);
                A.CallTo(() => this.fakeSessionIdentificationMethod.GetCurrentSessionId(this.nancyContext))
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionCache.Get(sessionId))
                    .MustHaveHappened();
            }

            [Fact]
            public void When_session_is_not_found_then_returns_new_empty_session()
            {
                var sessionId = new SessionId(Guid.NewGuid(), false);

                A.CallTo(() => this.fakeSessionIdentificationMethod.GetCurrentSessionId(this.nancyContext))
                    .Returns(sessionId);
                A.CallTo(() => this.fakeSessionCache.Get(sessionId))
                    .Returns(null);

                var actual = this.sessionManager.Load(this.nancyContext);

                Assert.NotNull(actual);
                Assert.IsNotType<InProcSession>(actual);
                Assert.Equal(0, actual.Count);
            }
        }

        public class Save : InProcSessionManagerFixture
        {
            private readonly ISession fakeSession;

            public Save()
            {
                this.fakeSession = A.Fake<ISession>();
                A.CallTo(() => this.fakeSession.Count)
                    .Returns(2);
                A.CallTo(() => this.fakeSession.HasChanged)
                    .Returns(true);
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(() => this.sessionManager.Save(this.fakeSession, null));
            }

            [Fact]
            public void Given_null_session_then_does_not_save()
            {
                this.sessionManager.Save(null, this.nancyContext);

                A.CallTo(() => this.fakeSessionCache.Set(A<InProcSession>._))
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdentificationMethod.SaveSessionId(A<SessionId>._, A<NancyContext>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void Given_unchanged_session_then_does_not_save()
            {
                A.CallTo(() => this.fakeSession.HasChanged)
                    .Returns(false);

                this.sessionManager.Save(this.fakeSession, this.nancyContext);

                A.CallTo(() => this.fakeSessionCache.Set(A<InProcSession>._))
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdentificationMethod.SaveSessionId(A<SessionId>._, A<NancyContext>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_no_session_is_present_in_context_then_does_not_save()
            {
                this.sessionManager.Save(new NullSessionProvider(), this.nancyContext);

                A.CallTo(() => this.fakeSessionCache.Set(A<InProcSession>._))
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdentificationMethod.SaveSessionId(A<SessionId>._, A<NancyContext>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_no_data_is_present_in_session_then_does_not_save()
            {
                A.CallTo(() => this.fakeSession.Count)
                    .Returns(0);

                this.sessionManager.Save(this.fakeSession, this.nancyContext);

                A.CallTo(() => this.fakeSessionCache.Set(A<InProcSession>._))
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdentificationMethod.SaveSessionId(A<SessionId>._, A<NancyContext>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void Given_valid_session_then_caches_that_session()
            {
                var sessionId = new SessionId(Guid.NewGuid(), true);
                var sessionToSave = new InProcSession(sessionId, this.fakeSession, DateTime.Now,
                    TimeSpan.FromMinutes(10));
                A.CallTo(() => this.fakeSessionIdentificationMethod.GetCurrentSessionId(this.nancyContext))
                    .Returns(sessionId);
                A.CallTo(() => this.fakeSessionFactory.Create(sessionId, this.fakeSession))
                    .Returns(sessionToSave);

                this.sessionManager.Save(this.fakeSession, this.nancyContext);
                A.CallTo(() => this.fakeSessionCache.Set(sessionToSave))
                    .MustHaveHappened();
            }

            [Fact]
            public void Given_valid_session_then_saves_that_session_using_method_from_configuration()
            {
                var sessionId = new SessionId(Guid.NewGuid(), true);
                A.CallTo(() => this.fakeSessionIdentificationMethod.GetCurrentSessionId(this.nancyContext))
                    .Returns(sessionId);

                this.sessionManager.Save(this.fakeSession, this.nancyContext);
                A.CallTo(() => this.fakeSessionIdentificationMethod.SaveSessionId(sessionId, this.nancyContext))
                    .MustHaveHappened();
            }
        }
    }
}