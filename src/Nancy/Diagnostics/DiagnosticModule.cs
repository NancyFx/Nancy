namespace Nancy.Diagnostics
{
    public abstract class DiagnosticModule : NancyModule
    {
        public DiagnosticModule()
            : base(DiagnosticsHook.ControlPanelPrefix)
        {
            
        }
    }
}