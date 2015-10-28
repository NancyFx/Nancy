namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System;
    using System.Threading;

    internal abstract class HeldLock : IHeldLock
    {
        protected readonly ReaderWriterLockSlim wrappedLock;

        protected HeldLock(ReaderWriterLockSlim wrappedLock)
        {
            if (wrappedLock == null)
            {
                throw new ArgumentNullException("wrappedLock");
            }
            this.wrappedLock = wrappedLock;
            this.Acquire();
        }

        public abstract void Dispose();

        protected abstract void Acquire();
    }
}