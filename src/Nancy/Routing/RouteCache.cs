namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Culture;

    /// <summary>
    /// Caches information about all the available routes that was discovered by the bootstrapper.
    /// </summary>
    public class RouteCache : Dictionary<Type, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        private readonly IRouteSegmentExtractor routeSegmentExtractor;
        private readonly IRouteDescriptionProvider routeDescriptionProvider;
        private readonly IEnumerable<IRouteMetadataProvider> routeMetadataProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteCache"/> class.
        /// </summary>
        /// <param name="moduleCatalog">The <see cref="INancyModuleCatalog"/> that should be used by the cache.</param>
        /// <param name="contextFactory">The <see cref="INancyContextFactory"/> that should be used to create a context instance.</param>
        /// <param name="routeSegmentExtractor"> </param>
        /// <param name="cultureService"></param>
        /// <param name="routeMetadataProviders"></param>
        /// <param name="routeDescriptionProvider"></param>
        public RouteCache(
            INancyModuleCatalog moduleCatalog,
            INancyContextFactory contextFactory,
            IRouteSegmentExtractor routeSegmentExtractor,
            IRouteDescriptionProvider routeDescriptionProvider,
            ICultureService cultureService,
            IEnumerable<IRouteMetadataProvider> routeMetadataProviders)
        {
            this.routeSegmentExtractor = routeSegmentExtractor;
            this.routeDescriptionProvider = routeDescriptionProvider;
            this.routeMetadataProviders = routeMetadataProviders;

            var request = new Request("GET", "/", "http");

            using (var context = contextFactory.Create(request))
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
            return !this.Values.SelectMany(r => r).Any();
        }

        private void BuildCache(IEnumerable<INancyModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleType = module.GetType();

                var routes =
                    module.Routes.Select(r => r.Description).ToArray();

                foreach (var routeDescription in routes)
                {
                    routeDescription.Description = this.routeDescriptionProvider.GetDescription(module, routeDescription.Path);
                    routeDescription.Segments = this.routeSegmentExtractor.Extract(routeDescription.Path).ToArray();
                    routeDescription.Metadata = this.GetRouteMetadata(module, routeDescription);
                }

                this.AddRoutesToCache(routes, moduleType);
            }
        }

        private RouteMetadata GetRouteMetadata(INancyModule module, RouteDescription routeDescription)
        {
            var data = new Dictionary<Type, object>();

            foreach (var provider in this.routeMetadataProviders)
            {
                var type = provider.GetMetadataType(module, routeDescription);
                var metadata = provider.GetMetadata(module, routeDescription);

                if (type != null && metadata != null)
                {
                    data.Add(type, metadata);
                }
            }

            return new RouteMetadata(data);
        }

        private void AddRoutesToCache(IEnumerable<RouteDescription> routes, Type moduleType)
        {
            if (!this.ContainsKey(moduleType))
            {
                this[moduleType] = new List<Tuple<int, RouteDescription>>();
            }

            this[moduleType].AddRange(routes.Select((r, i) => new Tuple<int, RouteDescription>(i, r)));
        }
    }
}