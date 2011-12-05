namespace Nancy.Diagnostics.Modules
{
    public class MainModule : DiagnosticModule
    {
        public MainModule()
        {
            Get["/"] = _ => "Dashboard!";
        }
    }
}