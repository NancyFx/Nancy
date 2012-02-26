namespace Nancy.Diagnostics
{
    public interface IDiagnosticsProvider
    {
        string Name { get; }

        string Description { get; }

        object DiagnosticObject { get; }
    }
}