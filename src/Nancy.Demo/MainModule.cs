namespace Nancy.Demo
{
    using Nancy.Demo.Models;
    using Nancy.Formatters;
    using Nancy.ViewEngines;
    using Nancy.ViewEngines.NDjango;
    using Nancy.ViewEngines.NHaml;
    using Nancy.ViewEngines.Razor;
    using Nancy.ViewEngines.Spark;

    public class Module : NancyModule
    {
        public Module()
        {
            Get["/"] = x => {
                return "This is the root! Visit <a href='/routes'>/routes</a> to see all registered routes!";
            };
            
            Get["/test"] = x => {
                return "Test";
            };

            Get["/routes"] = x => {
                var routes = GetRoutes("GET");
                return View.Razor("~/views/routes.cshtml", routes);
            };

            Get["/static"] = x => {
                return View.Static("~/views/static.htm");
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View.Razor("~/views/razor.cshtml", model);
            };
            Get["/nhaml"] = x => {
                var model = new RatPack { FirstName = "Andrew" };
                return View.Haml("~/views/nhaml.haml", model);
            };

            Get["/ndjango"] = x => {
                var model = new RatPack { FirstName = "Michael" };
                return View.Django("~/views/ndjango.django", model);
			};

            Get["/spark"] = x => {
                var model = new RatPack { FirstName = "Bright" };
                return View.Spark("~/views/spark.spark", model);
			};

            Get["/json"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsJson(model);
            };

            Get["/xml"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsXml(model);
            };
        }
    }
}
