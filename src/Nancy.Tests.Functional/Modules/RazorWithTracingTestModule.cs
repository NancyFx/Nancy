namespace Nancy.Tests.Functional.Modules
{
    public class RazorWithTracingTestModule : LegacyNancyModule
    {
        public RazorWithTracingTestModule()
        {
            Get["/tracing/razor-viewbag"] = _ =>
                {
                    this.ViewBag.Name = "Bob";

                    return View["RazorPage"];
                };
        }
    }
}
