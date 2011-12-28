namespace Nancy.Diagnostics.Modules
{
    public class InfoModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        public InfoModule(IRequestTracing sessionProvider)
            : base("/info")
        {
            this.sessionProvider = sessionProvider;

            Get["/"] = _ => View["Info"];
        }
    }
}