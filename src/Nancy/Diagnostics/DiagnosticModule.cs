namespace Nancy.Diagnostics
{
    public abstract class DiagnosticModule : NancyModule
    {
        protected DiagnosticModule()
            : base(DiagnosticsHook.ControlPanelPrefix)
        {
        }

        protected DiagnosticModule(string basePath)
            : base(DiagnosticsHook.ControlPanelPrefix + basePath)
        {
            
        }

        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(); }
        }
    }
}