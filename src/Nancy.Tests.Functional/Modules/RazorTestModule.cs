namespace Nancy.Tests.Functional.Modules
{
    public class RazorTestModule : NancyModule
    {
        public RazorTestModule()
        {
            Get["/razor-viewbag"] = _ =>
            {
                this.ViewBag.Name = "Bob";

                return View["RazorPage"];
            };
        }
    }
}