namespace Nancy.Session.InProc.InProcSessionsManagement
{
    internal interface IPeriodicCacheCleaner
    {
        void Start();
        void Stop();
    }
}