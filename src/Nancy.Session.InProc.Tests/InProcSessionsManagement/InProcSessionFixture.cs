namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using FakeItEasy;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Xunit;

    public class InProcSessionFixture
    {
        private readonly ISystemClock fakeSystemClock;
        private readonly ISession wrappedSession;

        public InProcSessionFixture()
        {
            this.wrappedSession = A.Fake<ISession>();
            this.fakeSystemClock = A.Fake<ISystemClock>();
        }

        [Fact]
        public void Given_null_id_fails()
        {
            Assert.Throws<ArgumentNullException>(
                () => new InProcSession(null, this.wrappedSession, DateTime.Now, TimeSpan.FromMinutes(15)));
        }

        [Fact]
        public void Given_empty_id_fails()
        {
            var emptySessionId = new SessionId(Guid.Empty, false);
            Assert.Throws<ArgumentException>(
                () => new InProcSession(emptySessionId, this.wrappedSession, DateTime.Now, TimeSpan.FromMinutes(15)));
        }

        [Fact]
        public void Given_null_wrapped_session_fails()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new InProcSession(new SessionId(Guid.NewGuid(), false), null, DateTime.Now, TimeSpan.FromMinutes(15)));
        }

        [Fact]
        public void Equals_other_session_with_same_id()
        {
            var sessionId = new SessionId(Guid.NewGuid(), false);
            var inProcSession1 = new InProcSession(sessionId, this.wrappedSession, DateTime.Now, TimeSpan.FromSeconds(3));
            var inProcSession2 = new InProcSession(sessionId, this.wrappedSession, DateTime.Now, TimeSpan.FromSeconds(3));

            var actual = inProcSession1.Equals(inProcSession2);

            Assert.True(actual);
        }

        [Fact]
        public void When_expired_isexpired_returns_true()
        {
            var creationTime = new DateTime(2015, 10, 20, 21, 19, 0, DateTimeKind.Utc);
            var timeout = TimeSpan.FromMinutes(10);
            this.ConfigureSystemClock_ToReturn(creationTime.AddMinutes(11));
            var inProcSession = new InProcSession(new SessionId(Guid.NewGuid(), false), this.wrappedSession,
                creationTime, timeout);

            var actual = inProcSession.IsExpired(this.fakeSystemClock.NowUtc);

            Assert.True(actual);
        }

        [Fact]
        public void When_not_expired_isexpired_returns_false()
        {
            var creationTime = new DateTime(2015, 10, 20, 21, 19, 0, DateTimeKind.Utc);
            var timeout = TimeSpan.FromMinutes(10);
            this.ConfigureSystemClock_ToReturn(creationTime.AddMinutes(2));
            var inProcSession = new InProcSession(new SessionId(Guid.NewGuid(), false), this.wrappedSession,
                creationTime, timeout);

            var actual = inProcSession.IsExpired(this.fakeSystemClock.NowUtc);

            Assert.False(actual);
        }

        [Fact]
        public void When_exactly_expiration_time_isexpired_returns_false()
        {
            var creationTime = new DateTime(2015, 10, 20, 21, 19, 0, DateTimeKind.Utc);
            var timeout = TimeSpan.FromMinutes(10.223);
            this.ConfigureSystemClock_ToReturn(creationTime.Add(timeout));
            var inProcSession = new InProcSession(new SessionId(Guid.NewGuid(), false), this.wrappedSession,
                creationTime, timeout);

            var actual = inProcSession.IsExpired(this.fakeSystemClock.NowUtc);

            Assert.False(actual);
        }

        private void ConfigureSystemClock_ToReturn(DateTime nowUtc)
        {
            A.CallTo(() => this.fakeSystemClock.NowUtc).Returns(nowUtc);
        }
    }
}