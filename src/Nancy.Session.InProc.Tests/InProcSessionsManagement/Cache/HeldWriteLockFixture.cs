namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.Cache
{
    using System;
    using System.Threading;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Xunit;

    public class HeldWriteLockFixture : IDisposable
    {
        private readonly ReaderWriterLockSlim wrappedLock;

        public HeldWriteLockFixture()
        {
            this.wrappedLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public void Dispose()
        {
            this.wrappedLock.Dispose();
        }

        [Fact]
        public void Given_null_readerwriterlock_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HeldWriteLock(null));
        }

        [Fact]
        public void When_creating_then_acquires_lock()
        {
            using (new HeldWriteLock(this.wrappedLock))
            {
                var actual = this.wrappedLock.IsWriteLockHeld;
                Assert.True(actual);
            }
        }

        [Fact]
        public void When_disposing_then_releases_lock()
        {
            using (new HeldWriteLock(this.wrappedLock))
            {
            }

            var actual = this.wrappedLock.IsWriteLockHeld;
            Assert.False(actual);
        }
    }
}