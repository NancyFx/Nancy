namespace Nancy.Diagnostics.Modules
{
    public class MainModule : DiagnosticModule
    {
        public MainModule()
        {
            Get("/", _ =>
            {
                return View["Dashboard"];
            });

            Post("/", _ =>
            {
                return this.Response.AsRedirect("~/");
            });
        }
    }
}
