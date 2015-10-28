namespace Nancy.Session.InProc.InProcSessionsManagement
{
    internal interface IHmacValidator
    {
        bool IsValidHmac(SessionIdentificationData sessionIdentificationData);
    }
}