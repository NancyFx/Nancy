namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;

    public class ViewEngineStartup : IStartup
    {
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IViewLocationCache viewLocationCache;
        private readonly IViewCache viewCache;

        public ViewEngineStartup(IEnumerable<IViewEngine> viewEngines, IViewLocationCache viewLocationCache, IViewCache viewCache)
        {
            this.viewEngines = viewEngines;
            this.viewLocationCache = viewLocationCache;
            this.viewCache = viewCache;
        }

        public void Initialize()
        {
            foreach (var viewEngine in viewEngines)
            {
                viewEngine.Initialize(CreateViewEngineStartupContext(viewEngine));
            }
        }

        private ViewEngineStartupContext CreateViewEngineStartupContext(IViewEngine viewEngine)
        {
            return new ViewEngineStartupContext(
                this.viewCache,
                GetViewsThatEngineCanRender(viewEngine));
        }

        private IEnumerable<ViewLocationResult> GetViewsThatEngineCanRender(IViewEngine viewEngine)
        {
            return viewEngine.Extensions.SelectMany(extension => this.viewLocationCache.Where(x => x.Extension.Equals(extension))).ToList();
        }
    }
}