namespace Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam
{
    internal interface ISessionIdentificationDataProvider
    {
        SessionIdentificationData ProvideDataFromQuery(Request request, string parameterName);
    }
}