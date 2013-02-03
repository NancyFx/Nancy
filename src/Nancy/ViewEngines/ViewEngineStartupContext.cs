namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Context passed to each view engine on startup
    /// </summary>
    public class ViewEngineStartupContext
    {
        public ViewEngineStartupContext(IViewCache viewCache, IViewLocator viewLocator, IEnumerable<string> extensions)
        {
            this.ViewLocator = viewLocator;
            this.Extensions = extensions;
            this.ViewCache = viewCache;
        }

        /// <summary>
        /// Gets the Nancy view cache - can be used to precompile views at startup
        /// if necessary.
        /// </summary>
        public IViewCache ViewCache { get; private set; }

        /// <summary>
        /// Gets the Nancy view locator
        /// </summary>
        public IViewLocator ViewLocator { get; private set; }

        /// <summary>
        /// Gets the extensions registered for the view engine
        /// </summary>
        public IEnumerable<string> Extensions { get; private set; }
    }
}