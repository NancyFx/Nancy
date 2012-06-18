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
        private readonly IViewLocationCache viewLocationCache;
        private readonly IViewCache viewCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEngineApplicationStartup"/> class, with the
        /// provided <paramref name="viewEngines"/>, <paramref name="viewLocationCache"/> and <paramref name="viewCache"/>.
        /// </summary>
        /// <param name="viewEngines">The available view engines.</param>
        /// <param name="viewLocationCache">The view location cache.</param>
        /// <param name="viewCache">The view cache.</param>
        public ViewEngineApplicationStartup(IEnumerable<IViewEngine> viewEngines, IViewLocationCache viewLocationCache, IViewCache viewCache)
        {
            this.viewEngines = viewEngines;
            this.viewLocationCache = viewLocationCache;
            this.viewCache = viewCache;
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
                this.viewLocationCache,
                viewEngine.Extensions);
        }
    }
}