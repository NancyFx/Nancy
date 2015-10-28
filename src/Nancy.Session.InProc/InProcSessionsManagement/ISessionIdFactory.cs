namespace Nancy.Session.InProc.InProcSessionsManagement
{
    internal interface ISessionIdFactory
    {
        SessionId CreateNew();

        SessionId CreateFrom(string sessionIdString);
    }
}