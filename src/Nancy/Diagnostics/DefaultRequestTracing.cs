namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultRequestTracing : IRequestTracing
    {
        private readonly object diagsLock = new object();

        private IList<RequestTraceSession> sessions = new List<RequestTraceSession>();

        public Guid CreateSession()
        {
            var id = Guid.NewGuid();

            lock (this.diagsLock)
            {
                this.sessions.Add(new RequestTraceSession(id));
            }

            return id;
        }

        // TODO - remove above method and return guid from here?
        public void AddRequestDiagnosticToSession(Guid sessionId, NancyContext context)
        {
            RequestTraceSession session;
            lock (this.diagsLock)
            {
                session = this.sessions.FirstOrDefault(s => s.Id == sessionId);
            }

            if (session == null)
            {
                return;
            }

            session.AddRequestTrace(context.Trace);
        }

        public IEnumerable<RequestTraceSession> GetSessions()
        {
            lock (this.diagsLock)
            {
                return this.sessions;
            }
        }

        public void Clear()
        {
            lock (this.diagsLock)
            {
                this.sessions.Clear();
            }
        }

        public bool IsValidSessionId(Guid sessionId)
        {
            lock (this.diagsLock)
            {
                return this.sessions.Any(s => s.Id == sessionId);
            }
        }
    }
}