namespace Nancy.Diagnostics.Modules
{
    public class InteractiveModule : DiagnosticModule
    {
        public InteractiveModule()
            :base ("/interactive")
        {
            Get["/"] = _ => View["InteractiveDiagnostics"];
        } 

    }
}