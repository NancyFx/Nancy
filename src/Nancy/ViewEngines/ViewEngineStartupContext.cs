namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    public class ViewEngineStartupContext
    {
        public ViewEngineStartupContext(IViewCache viewCache, IEnumerable<ViewLocationResult> viewLocationResults)
        {
            this.ViewCache = viewCache;
            this.ViewLocationResults = viewLocationResults;
        }

        public IViewCache ViewCache { get; private set; }

        public IEnumerable<ViewLocationResult> ViewLocationResults { get; private set; }
    }
}