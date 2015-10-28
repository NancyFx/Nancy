namespace Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie
{
    /// <summary>
    /// Identification method for in-process memory based sessions.
    /// </summary>
    public interface IBySessionIdCookieIdentificationMethod : IInProcSessionIdentificationMethod
    {
        /// <summary>
        /// Gets or sets the cookie name in which the session id is stored.
        /// </summary>
        string CookieName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain of the session cookie.
        /// </summary>
        string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the session cookie.
        /// </summary>
        string Path
        {
            get;
            set;
        }
    }
}