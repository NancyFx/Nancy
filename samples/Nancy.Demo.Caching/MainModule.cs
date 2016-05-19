namespace Nancy.Demo.Caching
{
    using System;
    using Nancy.Demo.Caching.CachingExtensions;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get("/", args => {
                return View["Index.cshtml", DateTime.Now.ToString()];
            });

            Get("/cached", args => {
                this.Context.EnableOutputCache(30);
                return View["Payload.cshtml", DateTime.Now.ToString()];
            });

            Get("/uncached", args => {
                this.Context.DisableOutputCache();
                return View["Payload.cshtml", DateTime.Now.ToString()];
            });
        }
    }
}