namespace Nancy.Diagnostics
{
    public abstract class DiagnosticModule : NancyModule
    {
        protected DiagnosticModule()
            : base(DiagnosticsHook.ControlPanelPrefix)
        {
        }

        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(); }
        }
    }
}