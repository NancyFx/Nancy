namespace Nancy.Html5BoilerPlateTemplate.Modules
{
    using Nancy;
    using Nancy.Formatters;
    using Nancy.ViewEngines;

    public class Html5BoilerPlateTestModule:NancyModule
    {

        public Html5BoilerPlateTestModule()
        {
            Get["/html5boilerplateTest/index"] = x =>
            {
                return View.Static("~/html5boilerplateTest/index.html");
            };

            Get["/html5boilerplateTest/{name}"] = x =>
            {
                return View.Static(string.Concat("~/html5boilerplateTest/", (string)x.name));
            };

          
        }
    }
}