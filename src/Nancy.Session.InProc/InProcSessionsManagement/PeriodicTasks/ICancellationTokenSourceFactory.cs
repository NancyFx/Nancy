namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System.Threading;

    internal interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource Create();
    }
}