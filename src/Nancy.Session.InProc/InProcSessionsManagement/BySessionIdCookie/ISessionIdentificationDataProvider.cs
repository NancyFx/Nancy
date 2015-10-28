namespace Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie
{
    internal interface ISessionIdentificationDataProvider
    {
        SessionIdentificationData ProvideDataFromCookie(Request request, string cookieName);
    }
}