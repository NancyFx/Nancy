namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using FakeItEasy;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Xunit;

    public class InProcSessionFactoryFixture
    {
        private readonly ISystemClock fakeSystemClock;
        private readonly InProcSessionFactory sessionFactory;
        private readonly InProcSessionsConfiguration validConfiguration;

        public InProcSessionFactoryFixture()
        {
            this.fakeSystemClock = A.Fake<ISystemClock>();
            this.validConfiguration = new InProcSessionsConfiguration
            {
                SessionIdentificationMethod = A.Dummy<IInProcSessionIdentificationMethod>(),
                SessionTimeout = TimeSpan.FromMinutes(30)
            };
            this.sessionFactory = new InProcSessionFactory(this.validConfiguration, this.fakeSystemClock);
        }

        [Fact]
        public void Given_null_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionFactory(null, this.fakeSystemClock));
        }

        [Fact]
        public void Given_null_system_clock_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionFactory(this.validConfiguration, null));
        }

        [Fact]
        public void Given_null_session_id_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.sessionFactory.Create(null, A.Dummy<ISession>()));
        }

        [Fact]
        public void Given_empty_session_id_then_throws()
        {
            var emptySessionId = new SessionId(Guid.Empty, false);
            Assert.Throws<ArgumentException>(() => this.sessionFactory.Create(emptySessionId, A.Dummy<ISession>()));
        }

        [Fact]
        public void Given_null_inner_session_then_throws()
        {
            var sessionId = new SessionId(Guid.NewGuid(), false);
            Assert.Throws<ArgumentNullException>(() => this.sessionFactory.Create(sessionId, null));
        }

        [Fact]
        public void Creates_session_with_expected_values()
        {
            var sessionId = new SessionId(Guid.NewGuid(), false);
            var innerSession = A.Dummy<ISession>();
            var nowUtc = new DateTime(2015, 10, 22, 16, 23, 14);
            this.ConfigureSystemClock_ToReturn(nowUtc);

            var actual = this.sessionFactory.Create(sessionId, innerSession);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<InProcSession>(actual);
            Assert.Equal(sessionId, actual.Id);
            Assert.Equal(nowUtc, actual.LastSave);
            Assert.Equal(this.validConfiguration.SessionTimeout, actual.Timeout);
            Assert.Equal(innerSession, actual.WrappedSession);
        }

        private void ConfigureSystemClock_ToReturn(DateTime nowUtc)
        {
            A.CallTo(() => this.fakeSystemClock.NowUtc).Returns(nowUtc);
        }
    }
}