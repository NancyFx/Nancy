namespace Nancy.Diagnostics.Modules
{
    public class TraceModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        public TraceModule(IRequestTracing sessionProvider)
            : base("/trace")
        {
            this.sessionProvider = sessionProvider;

            Get["/"] = _ => View["RequestTracing"];
        }
    }
}