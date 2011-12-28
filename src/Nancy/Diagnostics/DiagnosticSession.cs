namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class DiagnosticSession
    {
        private readonly object diagsLock = new object();

        private readonly List<RequestDiagnostic> requestDiagnostics;

        public Guid Id { get; private set; }

        public IEnumerable<RequestDiagnostic> RequestDiagnostics
        {
            get
            {
                return this.requestDiagnostics;
            }
        }

        public DiagnosticSession(Guid id)
        {
            this.Id = id;
            this.requestDiagnostics = new List<RequestDiagnostic>();
        }

        public void AddDiagnostic(RequestDiagnostic diagnostic)
        {
            lock (this.diagsLock)
            {
                this.requestDiagnostics.Add(diagnostic);
            }
        }
    }
}