namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    public interface IInteractiveDiagnostics
    {
        IEnumerable<InteractiveDiagnostic> AvailableDiagnostics { get; }

        object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnosticMethod, object[] arguments);
        
        string GetTemplate(InteractiveDiagnosticMethod interactiveDiagnosticMethod);
    }
}