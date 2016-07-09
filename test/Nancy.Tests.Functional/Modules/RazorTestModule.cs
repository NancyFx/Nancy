namespace Nancy.Tests.Functional.Modules
{
    public class RazorTestModule : NancyModule
    {
        public RazorTestModule()
        {
            Get("/razor-viewbag", args =>
            {
                this.ViewBag.Name = "Bob";

                return View["RazorPage"];
            });

            Get("/razor-viewbag-serialized", args =>
            {
                this.ViewBag.Name = "Bob";

                var serialized = this.ViewBag.ToDictionary();

                return serialized;
            });

            Get("/razor-partialnotfound", args =>
            {
                this.ViewBag.Name = "Bob";

                return View["RazorPageWithUnknownPartial"];
            });
        }
    }
}
