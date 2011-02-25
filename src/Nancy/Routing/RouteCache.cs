namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using System;

    public class RouteCache : Dictionary<string, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        private readonly IModuleKeyGenerator moduleKeyGenerator;

<<<<<<< HEAD
        public RouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;

            this.BuildCache(moduleCatalog.GetAllModules());
=======
        public RouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator, INancyContextFactory contextFactory)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;

            this.BuildCache(moduleCatalog.GetAllModules(contextFactory.Create()));
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
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
