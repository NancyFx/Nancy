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

            Get["/razor-viewbag-serialized"] = _ =>
            {
                this.ViewBag.Name = "Bob";

                var serialized = this.ViewBag.ToDictionary();

                return serialized;
            };
        }
    }
}
