namespace Nancy.Diagnostics.Modules
{
    public class MainModule : DiagnosticModule
    {
        public MainModule()
        {
            Get["/"] = _ => View["Dashboard"];

            Post["/"] = _ => Response.AsRedirect("~" + DiagnosticsHook.ControlPanelPrefix);
        }
    }
}