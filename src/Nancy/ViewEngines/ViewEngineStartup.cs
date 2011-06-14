namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    public class ViewEngineStartup : IStartup
    {
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IViewLocationCache viewLocationCache;

        public ViewEngineStartup(IEnumerable<IViewEngine> viewEngines, IViewLocationCache viewLocationCache)
        {
            this.viewEngines = viewEngines;
            this.viewLocationCache = viewLocationCache;
        }

        public void Initialize()
        {
            foreach (var viewEngine in viewEngines)
            {
                viewEngine.Initialize(GetViewsThatEngineCanRender(viewEngine));
            }
        }

        private IEnumerable<ViewLocationResult> GetViewsThatEngineCanRender(IViewEngine viewEngine)
        {
            return this.viewLocationCache.GetMatchingViews(x => viewEngine.Extensions.Any(ext => ext.Equals(x.Extension, StringComparison.OrdinalIgnoreCase)));
        }
    }
}