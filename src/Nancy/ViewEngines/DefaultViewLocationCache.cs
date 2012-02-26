using Nancy.Diagnostics;

namespace Nancy.ViewEngines
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultViewLocationCache : IViewLocationCache, IDiagnosticsProvider
    {
        private readonly IViewLocationProvider viewLocationProvider;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IEnumerable<ViewLocationResult> locatedViews;

        public DefaultViewLocationCache(IViewLocationProvider viewLocationProvider, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocationProvider = viewLocationProvider;
            this.viewEngines = viewEngines;
            this.locatedViews = GetLocatedViews();
        }

        private IEnumerable<ViewLocationResult> GetLocatedViews()
        {
            var supportedViewExtensions =
                GetSupportedViewExtensions();

            var viewsLocatedByProviders = 
                this.viewLocationProvider.GetLocatedViews(supportedViewExtensions);

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
            return StaticConfiguration.DisableCaches ? this.GetLocatedViews().GetEnumerator() : this.locatedViews.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Name
        {
            get { return "View location cache"; }
        }

        public string Description
        {
            get
            {
                return "Provides methods for viewing and manipulating the view cache.";
            }
        }

        public object DiagnosticObject
        {
            get { return new DefaultViewLocationCacheDiagnostics(this); }
        }

        public class DefaultViewLocationCacheDiagnostics
        {
            private readonly DefaultViewLocationCache cache;

            public DefaultViewLocationCacheDiagnostics(DefaultViewLocationCache cache)
            {
                this.cache = cache;
            }

            public IEnumerable<object> GetAllViews()
            {
                var x = this.cache.GetLocatedViews().Select(v => new
                                                                {
                                                                    v.Name,
                                                                    v.Location,
                                                                    v.Extension
                                                                }).ToArray();

                return x;
            }
        }
    }
}