namespace Nancy.Diagnostics.Modules
{
    public class MainModule : DiagnosticModule
    {
        public MainModule()
        {
            Get["/"] = async (_, __) =>
            {
                return View["Dashboard"];
            };

            Post["/"] = async (_, __) => this.Response.AsRedirect("~/");
        }
    }
}
