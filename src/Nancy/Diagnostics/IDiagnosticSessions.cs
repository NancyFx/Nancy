namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IDiagnosticSessions
    {
        Guid CreateSession();

        void AddRequestDiagnosticToSession(Guid id, NancyContext context);

        IEnumerable<DiagnosticSession> GetSessions();

        void Clear();
    }

    public class DefaultDiagnosticSessions : IDiagnosticSessions
    {
        private IList<DiagnosticSession> sessions = new List<DiagnosticSession>();

        public Guid CreateSession()
        {
            var id = Guid.NewGuid();

            this.sessions.Add(new DiagnosticSession(id));

            return id;
        }

        // TODO - remove above method and return guid from here?
        public void AddRequestDiagnosticToSession(Guid id, NancyContext context)
        {
            var diagnostic = this.sessions.FirstOrDefault(s => s.Id == id);

            if (diagnostic == null)
            {
                return;
            }

            diagnostic.RequestDiagnostics.Add(context.Diagnostic);
        }

        public IEnumerable<DiagnosticSession> GetSessions()
        {
            return this.sessions;
        }

        public void Clear()
        {
            this.sessions.Clear();
        }
    }
}