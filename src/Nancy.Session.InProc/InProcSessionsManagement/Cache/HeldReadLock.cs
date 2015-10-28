namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System.Threading;

    internal class HeldReadLock : HeldLock
    {
        public HeldReadLock(ReaderWriterLockSlim wrappedLock) : base(wrappedLock)
        {
        }

        protected override void Acquire()
        {
            this.wrappedLock.EnterReadLock();
        }

        public override void Dispose()
        {
            if (this.wrappedLock.IsReadLockHeld)
            {
                this.wrappedLock.ExitReadLock();
            }
        }
    }
}