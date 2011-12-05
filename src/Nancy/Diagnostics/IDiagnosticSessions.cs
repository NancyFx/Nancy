namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public interface IDiagnosticSessions
    {
        Guid CreateSession();

        void AddRequestDiagnosticToSession(Guid id, NancyContext context);

        IEnumerable<DiagnosticSession> GetSessions();

        void Clear();
    }
}