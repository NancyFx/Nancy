namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Linq;
    using Nancy.Configuration;
    using Nancy.Demo.Hosting.Aspnet.Metadata;
    using Nancy.Demo.Hosting.Aspnet.Models;
    using Nancy.Routing;
    using Nancy.Security;

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider, INancyEnvironment environment)
        {
            Get("/", args =>
            {
                return View["routes", routeCacheProvider.GetCache()];
            });

            Get("/texts", args =>
            {
                return (string)this.Context.Text.Menu.Home;
            });

            Get("/env", args =>
            {
                return "From nancy environment: " + environment.GetValue<MyConfig>().Value;
            });

            Get("/meta", args =>
            {
                return Negotiate
                    .WithModel(routeCacheProvider.GetCache().RetrieveMetadata<MyRouteMetadata>())
                    .WithView("meta");
            });

            Get("/uber-meta", args =>
            {
                return Negotiate
                    .WithModel(routeCacheProvider.GetCache().RetrieveMetadata<MyUberRouteMetadata>().OfType<MyUberRouteMetadata>())
                    .WithView("uber-meta");
            });

            Get("/text", args =>
            {
                var value = (string)this.Context.Text.Menu.Home;
                return string.Concat("Value of 'Home' resource key in the Menu resource file: ", value);
            });

            Get("/negotiated", args =>
            {
                return Negotiate
                    .WithModel(new RatPack { FirstName = "Nancy " })
                    .WithMediaRangeModel("text/html", new RatPack { FirstName = "Nancy fancy pants" })
                    .WithView("negotiatedview")
                    .WithHeader("X-Custom", "SomeValue");
            });

            Get("/user/{name}", args =>
            {
                return (string)args.name;
            });

            Get("/filtered",
                condition: r => true,
                action: args =>
                {
                    return "This is a route with a filter that always returns true.";
                }
            );

            Get("/filtered",
                condition: r => false,
                action: args =>
                {
                    return "This is also a route, but filtered out so should never be hit.";
                }
            );

            Get(@"/(?<foo>\d{2,4})/{bar}", args =>
            {
                return string.Format("foo: {0}<br/>bar: {1}", args.foo, args.bar);
            });

            Get("/test", args =>
            {
                return "Test";
            });

            Get("/nustache", args =>
            {
                return View["nustache", new { name = "Nancy", value = 1000000 }];
            });

            Get("/dotliquid", args =>
            {
                return View["dot", new { name = "dot" }];
            });

            Get("/javascript", args =>
            {
                return View["javascript.html"];
            });

            Get("/static", args =>
            {
                return View["static"];
            });

            Get("/razor", args =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor.cshtml", model];
            });

            Get("/razor-divzero", args =>
            {
                var model = new { FirstName = "Frank", Number = 22 };
                return View["razor-divzero.cshtml", model];
            });

            Get("/razorError", args =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-error.cshtml", model];
            });

            Get("/razor-simple", args =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-simple.cshtml", model];
            });

            Get("/razor-dynamic", args =>
            {
                return View["razor.cshtml", new { FirstName = "Frank" }];
            });

            Get("/razor-cs-strong", args =>
            {
                return View["razor-strong.cshtml", new RatPack { FirstName = "Frank" }];
            });

            Get("/razor-vb-strong", args =>
            {
                return View["razor-strong.vbhtml", new RatPack { FirstName = "Frank" }];
            });

            Get("/razor2", args =>
            {
                return new Razor2();
            });

            Get("/ssve", args =>
            {
                var model = new RatPack { FirstName = "You" };
                return View["ssve.sshtml", model];
            });

            Get("/viewmodelconvention", args =>
            {
                return View[new SomeViewModel()];
            });

            Get("/spark", args =>
            {
                var model = new RatPack { FirstName = "Bright" };
                return View["spark.spark", model];
            });

            Get("/spark-anon", args =>
            {
                var model = new { FirstName = "Anonymous" };
                return View["anon.spark", model];
            });

            Get("/json", args =>
            {
                var model = new RatPack { FirstName = "Andy" };
                return this.Response.AsJson(model);
            });

            Get("/xml", args =>
            {
                var model = new RatPack { FirstName = "Andy" };
                return this.Response.AsXml(model);
            });

            Get("/session", args =>
            {
                var value = Session["moo"] ?? "";

                var output = "Current session value is: " + value;

                if (string.IsNullOrEmpty(value.ToString()))
                {
                    Session["moo"] = "I've created a session!";
                }

                return output;
            });

            Get("/sessionObject", args =>
            {
                var value = Session["baa"] ?? "null";

                var output = "Current session value is: " + value;

                if (value.ToString() == "null")
                {
                    Session["baa"] = new Payload(27, true, "some random string value");
                }

                return output;
            });

            Get("/error", args =>
            {
                throw new NotSupportedException("This is an exception thrown in a route.");
                return 500;
            });

            Get("/customErrorHandler", args =>
            {
                return HttpStatusCode.ImATeapot;
            });

            Get("/csrf", args =>
            {
                return this.View["csrf", new { Blurb = "CSRF without an expiry using the 'session' token" }];
            });

            Post("/csrf", args =>
            {
                this.ValidateCsrfToken();

                return string.Format("Hello {0}!", this.Request.Form.Name);
            });

            Get("/csrfWithExpiry", args =>
            {
                // Create a new one because we have an expiry to check
                this.CreateNewCsrfToken();

                return this.View["csrf", new { Blurb = "You have 20 seconds to submit the page.. TICK TOCK :-)" }];
            });

            Post("/csrfWithExpiry", args =>
            {
                this.ValidateCsrfToken(TimeSpan.FromSeconds(20));

                return string.Format("Hello {0}!", this.Request.Form.Name);
            });

            Get("/viewNotFound", args => {
                return View["I-do-not-exist"];
            });

            Get("/fileupload", args =>
            {
                return View["FileUpload", new { Posted = "Nothing" }];
            });

            Post("/fileupload", args =>
            {
                var file = this.Request.Files.FirstOrDefault();

                string fileDetails = "Nothing";

                if (file != null)
                {
                    fileDetails = string.Format("{3} - {0} ({1}) {2}bytes", file.Name, file.ContentType, file.Value.Length, file.Key);
                }

                return View["FileUpload", new { Posted = fileDetails }];
            });

            Get(
                name: "NamedRoute",
                path: "/namedRoute",
                action: args =>
                {
                    return "I am a named route!";
                });
        }
    }
}
