namespace Nancy.Demo
{
    using Nancy.Demo.Models;
    using Nancy.ViewEngines;
    using Nancy.Routing;
    using System.Linq;

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = x => {
                return View.Razor("~/views/routes.cshtml", routeCacheProvider.GetCache().SelectMany(c => c.Value));
            };

            Get["/style/{file}"] = x => {
                return Response.AsCss("~/Content/" + (string)x.file);
            };

            Get["/scripts/{file}"] = x => {
                return Response.AsJs("~/Content/" + (string)x.file);
            };

            Get["/filtered", r => true] = x => {
                return "This is a route with a filter that always returns true.";
            };

            Get["/filtered", r => false] = x => {
                return "This is also a route, but filtered out so should never be hit.";
            };

            Get[@"/(?<foo>\d{2,4})/{bar}"] = x => {
                return string.Format("foo: {0}<br/>bar: {1}", x.foo, x.bar);
            };

            Get["/test"] = x => {
                return "Test";
            };

            Get["/javascript"] = x => {
                return View.Static("~/views/javascript.html");
            };

            Get["/static"] = x => {
                return View("~/views/static.htm");
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View("~/views/razor.cshtml", model);
            };

            Get["/ndjango"] = x => {
                var model = new RatPack { FirstName = "Michael" };
                return View("~/views/ndjango.django", model);
            };

            Get["/spark"] = x => {
                var model = new RatPack { FirstName = "Bright" };
                return View("~/views/spark.spark", model);
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
