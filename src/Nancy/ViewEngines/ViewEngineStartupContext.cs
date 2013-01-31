namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;

    public class ViewEngineStartupContext
    {
        private readonly IViewLocator viewLocator;

        public ViewEngineStartupContext(IViewCache viewCache, IViewLocator viewLocator, IEnumerable<string> extensions)
        {
            this.viewLocator = viewLocator;
            this.Extensions = extensions;
            this.ViewCache = viewCache;
        }

        public IViewCache ViewCache { get; private set; }

        public IEnumerable<string> Extensions { get; private set; }
    }
}