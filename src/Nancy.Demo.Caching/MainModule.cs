namespace Nancy.Demo.Caching
{
    using System;
    using Nancy;
    using Nancy.Demo.Caching.CachingExtensions;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x => {
                return View["Index.cshtml", DateTime.Now.ToString()];
            };

            Get["/cached"] = x => {
                Context.EnableOutputCache(30);
                return View["Payload.cshtml", DateTime.Now.ToString()];
            };

            Get["/uncached"] = x => {
                Context.DisableOutputCache();
                return View["Payload.cshtml", DateTime.Now.ToString()];
            };
        }
    }
}