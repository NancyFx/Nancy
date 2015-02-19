namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Calls the initialize method on all <see cref="IViewEngine"/> implementations, at application startup.
    /// </summary>
    public class ViewEngineApplicationStartup : IApplicationStartup
    {
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IViewCache viewCache;
        private readonly IViewLocator viewLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEngineApplicationStartup"/> class, with the
        /// provided <paramref name="viewEngines"/>, <paramref name="viewCache"/> and <paramref name="viewLocator"/>.
        /// </summary>
        /// <param name="viewEngines">The available view engines.</param>
        /// <param name="viewCache">The view cache.</param>
        /// <param name="viewLocator">The view locator.</param>
        public ViewEngineApplicationStartup(IEnumerable<IViewEngine> viewEngines, IViewCache viewCache, IViewLocator viewLocator)
        {
            this.viewEngines = viewEngines;
            this.viewCache = viewCache;
            this.viewLocator = viewLocator;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
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
                this.viewLocator);
        }
    }
}