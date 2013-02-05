namespace Nancy.Tests.Functional.Modules
{
    using Nancy.Tests.Functional.Models;

    public class RazorTestModule : NancyModule
    {
        public RazorTestModule()
        {
            Get["/razor-viewbag"] = _ =>
                {
                    this.ViewBag.Name = "Bob";

                    return View["RazorPage"];
                };

            // "Model" gets stripped for view location so should find a view called "My"
            Get["/just-model"] = _ => new MyModel { Name = "Bob" };
        }
    }
}
