namespace Nancy.Diagnostics
{
    public interface IDiagnosticsProvider
    {
        string Name { get; }

        object DiagnosticObject { get; }
    }
}