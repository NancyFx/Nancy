namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;

    public class ViewEngineStartupContext
    {
        public ViewEngineStartupContext(IViewCache viewCache, IViewLocator viewLocator, IEnumerable<string> extensions)
        {
            this.ViewLocator = viewLocator;
            this.Extensions = extensions;
            this.ViewCache = viewCache;
        }

        public IViewCache ViewCache { get; private set; }

        public IViewLocator ViewLocator { get; private set; }

        public IEnumerable<string> Extensions { get; private set; }
    }
}