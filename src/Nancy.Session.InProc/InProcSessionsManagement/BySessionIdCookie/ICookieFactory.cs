namespace Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie
{
    using Nancy.Cookies;

    internal interface ICookieFactory
    {
        INancyCookie CreateCookie(string cookieName,
            string cookieDomain,
            string cookiePath,
            SessionIdentificationData sessionIdentificationData);
    }
}