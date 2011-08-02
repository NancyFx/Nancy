namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class DiagnosticSession
    {
        public Guid Id { get; private set; }

        public IList<RequestDiagnostic> RequestDiagnostics { get; private set; }

        public DiagnosticSession(Guid id)
        {
            this.Id = id;
            this.RequestDiagnostics = new List<RequestDiagnostic>();
        }
    }
}