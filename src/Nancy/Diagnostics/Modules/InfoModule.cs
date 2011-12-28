namespace Nancy.Diagnostics.Modules
{
    public class SettingsModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        public SettingsModule(IRequestTracing sessionProvider)
            : base("/settings")
        {
            this.sessionProvider = sessionProvider;

            Get["/"] = _ => View["Settings"];
        }
    }
}