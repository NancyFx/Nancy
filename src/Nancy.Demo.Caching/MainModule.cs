namespace Nancy.Demo.Caching
{
    using System;

    using Nancy.Demo.Caching.CachingExtensions;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x => {
                return View["Index.cshtml", DateTime.Now.ToString()];
            };

            Get["/cached"] = x => {
                this.Context.EnableOutputCache(30);
                return View["Payload.cshtml", DateTime.Now.ToString()];
            };

            Get["/uncached"] = x => {
                this.Context.DisableOutputCache();
                return View["Payload.cshtml", DateTime.Now.ToString()];
            };
        }
    }
}