namespace Nancy.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Nancy.Demo.Models;
    using Nancy.Routing;

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = x => {
                return View["routes.cshtml", routeCacheProvider.GetCache()];
            };

            Get["/style/{file}"] = x => {
                return Response.AsCss("Content/" + (string)x.file);
            };

            Get["/scripts/{file}"] = x => {
                return Response.AsJs("Content/" + (string)x.file);
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
                return View["~/views/javascript.html"];
            };

            Get["/static"] = x => {
                return View["~/views/static.htm"];
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View["~/views/razor.cshtml", model];
            };

            Get["/embedded"] = x => {
                var model = new RatPack { FirstName = "Embedded" };
                return View["embedded", model];
            };

            Get["/embedded2"] = x => {
                var model = new RatPack { FirstName = "Embedded2" };
                return View["embedded.django", model];
            };

            Get["/viewmodelconvention"] = x => {
                return View[new SomeViewModel()];
            };

            Get["/ndjango"] = x => {
                var model = new RatPack { FirstName = "Michael" };
                return View["~/views/ndjango.django", model];
            };

            Get["/spark"] = x => {
                var model = new RatPack { FirstName = "Bright" };
                return View["~/views/spark.spark", model];
            };

            Get["/json"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsJson(model);
            };

            Get["/xml"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsXml(model);
            };

            Get["session"] = x =>
                { 
                    var output = "Current session value is: " + Session["moo"];

                    Session["moo"] = "I've created a session!";

                    return output;
                };
        }
    }
}
