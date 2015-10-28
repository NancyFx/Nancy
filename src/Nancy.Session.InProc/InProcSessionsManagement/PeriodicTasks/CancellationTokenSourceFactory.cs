namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System.Threading;

    internal class CancellationTokenSourceFactory : ICancellationTokenSourceFactory
    {
        public CancellationTokenSource Create()
        {
            return new CancellationTokenSource();
        }
    }
}