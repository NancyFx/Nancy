namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using System;

    public class RouteCache : Dictionary<string, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        private readonly IModuleKeyGenerator moduleKeyGenerator;

        public RouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator, INancyContextFactory contextFactory)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;

            using (var context = contextFactory.Create())
            {
                this.BuildCache(moduleCatalog.GetAllModules(context));
            }
        }

        private void BuildCache(IEnumerable<NancyModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleType = module.GetType();
                var moduleKey = this.moduleKeyGenerator.GetKeyForModuleType(moduleType);

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

        public bool IsEmpty()
        {
            return this.Values.SelectMany(r => r).Count() == 0;
        }
    }
}
