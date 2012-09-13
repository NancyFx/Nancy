namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using System;

    /// <summary>
    /// Caches information about all the available routes that was discovered by the bootstrapper.
    /// </summary>
    public class RouteCache : Dictionary<string, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        private readonly IModuleKeyGenerator moduleKeyGenerator;
        private readonly IRouteSegmentExtractor routeSegmentExtractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteCache"/> class.
        /// </summary>
        /// <param name="moduleCatalog">The <see cref="INancyModuleCatalog"/> that should be used by the cache.</param>
        /// <param name="moduleKeyGenerator">The <see cref="IModuleKeyGenerator"/> used to generate module keys.</param>
        /// <param name="contextFactory">The <see cref="INancyContextFactory"/> that should be used to create a context instance.</param>
        /// <param name="routeSegmentExtractor"> </param>
        public RouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator, INancyContextFactory contextFactory, IRouteSegmentExtractor routeSegmentExtractor)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;
            this.routeSegmentExtractor = routeSegmentExtractor;

            using (var context = contextFactory.Create())
            {
                this.BuildCache(moduleCatalog.GetAllModules(context));
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates of the cache is empty or not.
        /// </summary>
        /// <returns><see langword="true"/> if the cache is empty, otherwise <see langword="false"/>.</returns>
        public bool IsEmpty()
        {
            return this.Values.SelectMany(r => r).Count() == 0;
        }

        private void BuildCache(IEnumerable<NancyModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleType = module.GetType();
                var moduleKey = this.moduleKeyGenerator.GetKeyForModuleType(moduleType);

                var routes =
                    module.Routes.Select(r => r.Description);

                foreach (var routeDescription in routes)
                {
                    routeDescription.Segments = this.routeSegmentExtractor.Extract(routeDescription.Path);
                }

                this.AddRoutesToCache(module.Routes.Select(r => r.Description), moduleKey);
            }
        }

        private void AddRoutesToCache(IEnumerable<RouteDescription> routes, string moduleKey)
        {
            if (!this.ContainsKey(moduleKey))
            {
                this[moduleKey] = new List<Tuple<int, RouteDescription>>();
            }

            this[moduleKey].AddRange(routes.Select((r, i) => new Tuple<int, RouteDescription>(i, r)));
        }
    }
}
