namespace Nancy.Diagnostics.Modules
{
    public class SettingsModule : DiagnosticModule
    {
        public SettingsModule()
            : base("/settings")
        {
            Get["/"] = _ => View["Settings"];
        }
    }
}