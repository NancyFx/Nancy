namespace Nancy.Html5BoilerPlateTemplate.Modules
{
    using Nancy;
    using Nancy.Formatters;
    using Nancy.ViewEngines;

    public class Html5BoilerPlateDemoModule:NancyModule
    {

        public Html5BoilerPlateDemoModule()
        {

            Get["/html5boilerplateDemo/{name}"] = x =>
            {
                return View.Static(string.Concat("~/html5boilerplateDemo/", (string)x.name));
            };

        }
    }
}