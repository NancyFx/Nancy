namespace Nancy.Tests.Functional.Modules
{
    public class RazorTestModule : LegacyNancyModule
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

            Get["/razor-partialnotfound"] = _ =>
            {
                this.ViewBag.Name = "Bob";

                return View["RazorPageWithUnknownPartial"];
            };
        }
    }
}
