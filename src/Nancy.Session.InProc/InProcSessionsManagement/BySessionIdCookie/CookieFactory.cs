namespace Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cookies;

    internal class CookieFactory : ICookieFactory
    {
        public INancyCookie CreateCookie(string cookieName,
            string cookieDomain,
            string cookiePath,
            SessionIdentificationData sessionIdentificationData)
        {
            if (sessionIdentificationData == null)
            {
                throw new ArgumentNullException("sessionIdentificationData");
            }
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentNullException("cookieName");
            }

            return new NancyCookie(
                cookieName,
                sessionIdentificationData.ToString(),
                true)
            {
                Domain = cookieDomain,
                Path = cookiePath
            };
        }
    }
}