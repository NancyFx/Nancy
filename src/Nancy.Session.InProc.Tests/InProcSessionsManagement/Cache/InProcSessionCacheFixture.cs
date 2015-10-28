namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using FakeItEasy;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Xunit;

    public class InProcSessionCacheFixture : IDisposable
    {
        private readonly InProcSession activeSession;
        private readonly InProcSession expiredSession;
        private readonly ISystemClock fakeSystemClock;
        private readonly InProcSessionCache inProcSessionCache;
        private readonly DateTime nowUtc;
        private readonly int numberOfSessions;

        public InProcSessionCacheFixture()
        {
            this.nowUtc = new DateTime(2015, 10, 20, 21, 36, 14, DateTimeKind.Utc);
            this.fakeSystemClock = A.Fake<ISystemClock>();
            this.ConfigureSystemClock_ToReturn(this.nowUtc);

            this.inProcSessionCache = new InProcSessionCache(this.fakeSystemClock);
            this.expiredSession = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(),
                this.nowUtc.AddMinutes(-20),
                TimeSpan.FromMinutes(15));
            this.activeSession = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(),
                this.nowUtc.AddMinutes(-3),
                TimeSpan.FromMinutes(15));
            this.inProcSessionCache.Set(this.expiredSession);
            this.inProcSessionCache.Set(this.activeSession);
            this.numberOfSessions = 2;
        }

        public void Dispose()
        {
            this.inProcSessionCache.Dispose();
        }

        [Fact]
        public void Count_returns_number_of_elements()
        {
            var actual = this.inProcSessionCache.Count;
            Assert.Equal(this.numberOfSessions, actual);
        }

        [Fact]
        public void Trim_removes_expired_elements()
        {
            this.inProcSessionCache.Trim();

            var expected = this.numberOfSessions - 1;
            var actual = this.inProcSessionCache.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Set_adds_new_element()
        {
            var extraSession = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(), this.nowUtc,
                TimeSpan.FromMinutes(15));
            this.inProcSessionCache.Set(extraSession);

            var expected = this.numberOfSessions + 1;
            var actual = this.inProcSessionCache.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Set_replaces_item_when_element_already_is_cached()
        {
            var newSession = new InProcSession(
                this.activeSession.Id,
                A.Dummy<ISession>(),
                this.nowUtc.AddSeconds(10),
                TimeSpan.FromMinutes(10)
                );
            this.inProcSessionCache.Set(newSession);

            var actual = this.inProcSessionCache.Count;
            Assert.Equal(this.numberOfSessions, actual);

            var sessionAfterSave = this.inProcSessionCache.Get(this.activeSession.Id);
            Assert.Equal(sessionAfterSave.LastSave, newSession.LastSave);
        }

        [Fact]
        public void Get_gets_element_if_id_is_found()
        {
            var idToFind = this.activeSession.Id;
            var actual = this.inProcSessionCache.Get(idToFind);
            Assert.Equal(this.activeSession, actual);
        }

        [Fact]
        public void Get_returns_null_if_id_is_not_found()
        {
            var nonExistingId = new SessionId(Guid.NewGuid(), false);
            var actual = this.inProcSessionCache.Get(nonExistingId);
            Assert.Null(actual);
        }

        [Fact]
        public void Get_throws_when_null_session_id_is_specified()
        {
            Assert.Throws<ArgumentNullException>(() => this.inProcSessionCache.Get(null));
        }

        [Fact]
        public void Get_removes_value_if_it_is_expired_and_returns_null()
        {
            var idToFind = this.expiredSession.Id;
            var actualSession = this.inProcSessionCache.Get(idToFind);
            Assert.Null(actualSession);

            var expectedCount = this.numberOfSessions - 1;
            var actualCount = this.inProcSessionCache.Count;
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void When_disposed_then_cannot_access_Count()
        {
            this.inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => this.inProcSessionCache.Count);
        }

        [Fact]
        public void When_disposed_then_cannot_access_Get()
        {
            var idToFind = new SessionId(Guid.NewGuid(), false);
            this.inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => this.inProcSessionCache.Get(idToFind));
        }

        [Fact]
        public void When_disposed_then_cannot_access_Set()
        {
            var extraSession = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(), this.nowUtc,
                TimeSpan.FromMinutes(15));
            this.inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => this.inProcSessionCache.Set(extraSession));
        }

        [Fact]
        public void Collection_is_thread_safe()
        {
            const int numberOfThreads = 500;
            var threadAction = new ThreadStart(() =>
            {
                var extraSession1 = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(),
                    this.nowUtc.AddMinutes(-20),
                    TimeSpan.FromMinutes(15));
                var extraSession2 = new InProcSession(new SessionId(Guid.NewGuid(), false), A.Dummy<ISession>(),
                    this.nowUtc, TimeSpan.FromMinutes(15));
                this.inProcSessionCache.Set(extraSession1);
                Thread.Sleep(20);
                this.inProcSessionCache.Get(extraSession1.Id);
                this.inProcSessionCache.Set(extraSession2);
                this.inProcSessionCache.Get(extraSession2.Id);
            });

            var threads = new List<Thread>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var newThread = new Thread(threadAction);
                newThread.Start();
                threads.Add(newThread);
            }
            threads.ForEach(thread => thread.Join());

            var expectedNumberOfSessions = numberOfThreads + this.numberOfSessions;
            var actualNumberOfSessions = this.inProcSessionCache.Count;

            Assert.Equal(expectedNumberOfSessions, actualNumberOfSessions);
        }

        private void ConfigureSystemClock_ToReturn(DateTime nowUtc)
        {
            A.CallTo(() => this.fakeSystemClock.NowUtc).Returns(nowUtc);
        }
    }
}