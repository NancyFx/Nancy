namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public interface IRequestTracing
    {
        Guid CreateSession();

        void AddRequestDiagnosticToSession(Guid id, NancyContext context);

        IEnumerable<RequestTraceSession> GetSessions();

        void Clear();
    }
}