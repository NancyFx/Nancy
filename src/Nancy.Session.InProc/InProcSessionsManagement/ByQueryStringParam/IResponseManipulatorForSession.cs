namespace Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam
{
    internal interface IResponseManipulatorForSession
    {
        void ModifyResponseToRedirectToSessionAwareUrl(
            NancyContext context,
            SessionIdentificationData sessionIdentificationData,
            string parameterName);
    }
}