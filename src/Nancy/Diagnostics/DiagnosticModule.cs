namespace Nancy.Diagnostics
{
    using Configuration;

    public abstract class DiagnosticModule : NancyModule
    {
        private readonly INancyEnvironment environment;

        protected DiagnosticModule()
            : this(string.Empty)
        {
        }

        protected DiagnosticModule(string basePath)
            : base(basePath)
        {
            this.environment = new DefaultNancyEnvironment();
            this.environment.AddValue(ViewConfiguration.Default);
        }

        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(this.Context, this.environment); }
        }
    }
}
