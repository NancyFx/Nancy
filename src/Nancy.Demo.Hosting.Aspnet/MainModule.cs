namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Linq;

    using Nancy.Demo.Hosting.Aspnet.Metadata;
    using Nancy.Demo.Hosting.Aspnet.Models;
    using Nancy.Routing;
    using Nancy.Security;

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = x => {
                return View["routes", routeCacheProvider.GetCache()];
            };

            Get["/texts"] = parameters => {
                return (string)this.Context.Text.Menu.Home;
                
            };

            Get["/meta"] = parameters =>
            {
                return Negotiate
                    .WithModel(routeCacheProvider.GetCache().RetrieveMetadata<MyRouteMetadata>())
                    .WithView("meta");
            };

            Get["/uber-meta"] = parameters =>
            {
                return Negotiate
                    .WithModel(routeCacheProvider.GetCache().RetrieveMetadata<MyUberRouteMetadata>().OfType<MyUberRouteMetadata>())
                    .WithView("uber-meta");
            };

            Get["/text"] = x =>
            {
                var value = (string)this.Context.Text.Menu.Home;
                return string.Concat("Value of 'Home' resource key in the Menu resource file: ", value);
            };

            Get["/negotiated"] = parameters => {
                return Negotiate
                    .WithModel(new RatPack {FirstName = "Nancy "})
                    .WithMediaRangeModel("text/html", new RatPack {FirstName = "Nancy fancy pants"})
                    .WithView("negotiatedview")
                    .WithHeader("X-Custom", "SomeValue");
            };

            Get["/user/{name}"] = parameters =>
            {
                return (string)parameters.name;
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

            Get["/nustache"] = parameters => {
                return View["nustache", new { name = "Nancy", value = 1000000 }];
            };

            Get["/dotliquid"] = parameters => {
                return View["dot", new { name = "dot" }];
            };

            Get["/javascript"] = x => {
                return View["javascript.html"];
            };

            Get["/static"] = x => {
                return View["static"];
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor.cshtml", model];
            };

            Get["/razor-divzero"] = x =>
            {
                var model = new { FirstName = "Frank", Number = 22 };
                return View["razor-divzero.cshtml", model];
            };

            Get["/razorError"] = x =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-error.cshtml", model];
            };

            Get["/razor-simple"] = x =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-simple.cshtml", model];
            };

            Get["/razor-dynamic"] = x =>
            {
                return View["razor.cshtml", new { FirstName = "Frank" }];
            };

            Get["/razor-cs-strong"] = x =>
            {
                return View["razor-strong.cshtml", new RatPack { FirstName = "Frank" }];
            };

            Get["/razor-vb-strong"] = x =>
            {
                return View["razor-strong.vbhtml", new RatPack { FirstName = "Frank" }];
            };

            Get["/razor2"] = _ => new Razor2();

            Get["/ssve"] = x =>
            {
                var model = new RatPack { FirstName = "You" };
                return View["ssve.sshtml", model];
            };

            Get["/viewmodelconvention"] = x => {
                return View[new SomeViewModel()];
            };

            Get["/spark"] = x => {
                var model = new RatPack { FirstName = "Bright" };
                return View["spark.spark", model];
            };

            Get["/spark-anon"] = x =>
            {
                var model = new { FirstName = "Anonymous" };
                return View["anon.spark", model];
            };

            Get["/json"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return this.Response.AsJson(model);
            };

            Get["/xml"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return this.Response.AsXml(model);
            };

            Get["/session"] = x => {
                var value = Session["moo"] ?? "";

                var output = "Current session value is: " + value;

                if (String.IsNullOrEmpty(value.ToString()))
                {
                    Session["moo"] = "I've created a session!";
                }

                return output;
            };

            Get["/sessionObject"] = x => {
                var value = Session["baa"] ?? "null";

                var output = "Current session value is: " + value;

                if (value.ToString() == "null")
                {
                    Session["baa"] = new Payload(27, true, "some random string value");
                }

                return output;
            };

            Get["/error"] = x =>
                {
                    throw new NotSupportedException("This is an exception thrown in a route.");
                };

            Get["/customErrorHandler"] = _ => HttpStatusCode.ImATeapot;

            Get["/csrf"] = x => this.View["csrf", new { Blurb = "CSRF without an expiry using the 'session' token" }];

            Post["/csrf"] = x =>
            {
                this.ValidateCsrfToken();

                return string.Format("Hello {0}!", this.Request.Form.Name);
            };

            Get["/csrfWithExpiry"] = x =>
                {
                    // Create a new one because we have an expiry to check
                    this.CreateNewCsrfToken();

                    return this.View["csrf", new { Blurb = "You have 20 seconds to submit the page.. TICK TOCK :-)" }];
                };

            Post["/csrfWithExpiry"] = x =>
                {
                    this.ValidateCsrfToken(TimeSpan.FromSeconds(20));

                    return string.Format("Hello {0}!", this.Request.Form.Name);
                };

            Get["/viewNotFound"] = _ => View["I-do-not-exist"];

            Get["/fileupload"] = x =>
            {
                return View["FileUpload", new { Posted = "Nothing" }];
            };

            Post["/fileupload"] = x =>
            {
                var file = this.Request.Files.FirstOrDefault();

                string fileDetails = "Nothing";

                if (file != null)
                {
                    fileDetails = string.Format("{3} - {0} ({1}) {2}bytes", file.Name, file.ContentType, file.Value.Length, file.Key);
                }

                return View["FileUpload", new { Posted = fileDetails }];
            };

            Get["NamedRoute", "/namedRoute"] = _ => "I am a named route!";
        }
    }
}
