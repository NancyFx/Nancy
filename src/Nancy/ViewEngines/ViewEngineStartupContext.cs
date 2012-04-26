namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;

    public class ViewEngineStartupContext
    {
        private readonly IEnumerable<ViewLocationResult> viewLocationCache;
        private readonly IEnumerable<ViewLocationResult> availableViews;

        public ViewEngineStartupContext(IViewCache viewCache, IEnumerable<ViewLocationResult> viewLocationCache, IEnumerable<string> extensions)
        {
            this.viewLocationCache = viewLocationCache;
            this.Extensions = extensions;
            this.ViewCache = viewCache;
            this.availableViews = GetViewsThatEngineCanRender(this.viewLocationCache, this.Extensions);
        }

        public IViewCache ViewCache { get; private set; }

        public IEnumerable<ViewLocationResult> ViewLocationResults
        {
            get
            {
                return StaticConfiguration.DisableCaches
                           ? GetViewsThatEngineCanRender(this.viewLocationCache, this.Extensions)
                           : this.availableViews;
            }
        }

        public IEnumerable<string> Extensions { get; private set; }

        private static IEnumerable<ViewLocationResult> GetViewsThatEngineCanRender(IEnumerable<ViewLocationResult> viewLocationCache, IEnumerable<string> extensions)
        {
            return extensions.SelectMany(extension => viewLocationCache.Where(x => x.Extension.Equals(extension))).ToArray();
        }
    }
}