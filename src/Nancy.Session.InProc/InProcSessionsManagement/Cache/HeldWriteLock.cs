namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System.Threading;

    internal class HeldWriteLock : HeldLock
    {
        public HeldWriteLock(ReaderWriterLockSlim wrappedLock) : base(wrappedLock)
        {
        }

        protected override void Acquire()
        {
            this.wrappedLock.EnterWriteLock();
        }

        public override void Dispose()
        {
            if (this.wrappedLock.IsWriteLockHeld)
            {
                this.wrappedLock.ExitWriteLock();
            }
        }
    }
}