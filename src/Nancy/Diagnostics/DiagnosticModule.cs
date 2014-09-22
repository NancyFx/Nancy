namespace Nancy.Diagnostics
{
    public abstract class DiagnosticModule : NancyModule
    {
        protected DiagnosticModule()
        {
        }

        protected DiagnosticModule(string basePath)
            : base(basePath)
        {
        }

        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(this.Context); }
        }
    }
}