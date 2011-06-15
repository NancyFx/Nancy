namespace Nancy.ViewEngines
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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

        public IEnumerator<ViewLocationResult> GetEnumerator()
        {
            return this.locatedViews.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}