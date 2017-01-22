namespace Nancy.Diagnostics.Modules
{
    /// <summary>
    /// Main Nancy module for diagnostics.
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.DiagnosticModule" />
    public class MainModule : DiagnosticModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainModule"/> class.
        /// </summary>
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
