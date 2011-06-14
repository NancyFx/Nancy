namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IStartup
    {
        void Initialize();
    }

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

    public interface IViewLocationCache
    {
        IEnumerable<ViewLocationResult> GetMatchingViews(Func<ViewLocationResult, bool> criterion);
    }

    public class DefaultViewLocationCache : IViewLocationCache
    {
        private readonly IEnumerable<IViewLocationProvider> viewLocationProviders;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IEnumerable<ViewLocationResult> locatedViews;

        public DefaultViewLocationCache(IEnumerable<IViewLocationProvider> viewLocationProviders, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocationProviders = viewLocationProviders;
            this.viewEngines = viewEngines;
            this.locatedViews = GetLocatedViews();
        }

        public IEnumerable<ViewLocationResult> GetMatchingViews(Func<ViewLocationResult, bool> criterion)
        {
            return this.locatedViews.Where(criterion);
        }

        private IEnumerable<ViewLocationResult> GetLocatedViews()
        {
            var supportedViewExtensions =
                GetSupportedViewExtensions();

            var viewsLocatedByProviders = this.viewLocationProviders
                .SelectMany(x => x.GetLocatedViews(supportedViewExtensions))
                .ToList();

            return viewsLocatedByProviders;
        }

        private IEnumerable<string> GetSupportedViewExtensions()
        {
            return this.viewEngines
                .SelectMany(engine => engine.Extensions)
                .Distinct();
        }
    }
}