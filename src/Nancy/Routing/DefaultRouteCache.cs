namespace Nancy.Routing
{
    using System.Collections.Generic;
    using Nancy.BootStrapper;
    using Nancy.Extensions;

    public class DefaultRouteCache : IRouteCache
    {
        private readonly List<RouteCacheEntry> cache;
        private readonly IModuleKeyGenerator moduleKeyGenerator;

        public DefaultRouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;
            this.cache = new List<RouteCacheEntry>();

            this.BuildCache(moduleCatalog.GetAllModules());
        }

        private void BuildCache(IEnumerable<NancyModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleType = module.GetType();
                var moduleKey = this.moduleKeyGenerator.GetKeyForModuleType(moduleType);

                this.AddMethodRoutesToCache(module, moduleKey, "GET");
                this.AddMethodRoutesToCache(module, moduleKey, "POST");
                this.AddMethodRoutesToCache(module, moduleKey, "PUT");
                this.AddMethodRoutesToCache(module, moduleKey, "DELETE");
            }
        }

        public IEnumerator<RouteCacheEntry> GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        private void AddMethodRoutesToCache(NancyModule module, string moduleKey, string method)
        {
            foreach (var description in module.GetRouteDescription(method))
            {
                this.cache.Add(new RouteCacheEntry(moduleKey, method, description.Path, description.Condition));
            }
        }
    }
}
