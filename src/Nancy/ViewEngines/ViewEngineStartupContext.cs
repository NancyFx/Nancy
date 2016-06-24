namespace Nancy.ViewEngines
{
    /// <summary>
    /// Context passed to each view engine on startup
    /// </summary>
    public class ViewEngineStartupContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEngineStartupContext"/> class.
        /// </summary>
        /// <param name="viewCache">The view cache.</param>
        /// <param name="viewLocator">The view locator.</param>
        public ViewEngineStartupContext(IViewCache viewCache, IViewLocator viewLocator)
        {
            this.ViewLocator = viewLocator;
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
    }
}