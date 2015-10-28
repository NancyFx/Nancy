namespace Nancy.Session.InProc.InProcSessionsManagement
{
    using System;

    internal class SessionIdFactory : ISessionIdFactory
    {
        public SessionId CreateNew()
        {
            return new SessionId(Guid.NewGuid(), true);
        }

        public SessionId CreateFrom(string sessionIdString)
        {
            if (sessionIdString == null)
            {
                return null;
            }

            Guid sessionId;
            return Guid.TryParse(sessionIdString, out sessionId) ? new SessionId(sessionId, false) : null;
        }
    }
}