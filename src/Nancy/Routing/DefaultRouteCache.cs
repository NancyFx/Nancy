using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.BootStrapper;
using Nancy.Extensions;

namespace Nancy.Routing
{
    public class DefaultRouteCache : IRouteCache
    {
        private readonly List<RouteCacheEntry> _Cache;
        private readonly IModuleKeyGenerator _ModuleKeyGenerator;

        public DefaultRouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator)
        {
            _ModuleKeyGenerator = moduleKeyGenerator;
            _Cache = new List<RouteCacheEntry>();

            BuildCache(moduleCatalog.GetAllModules());
        }

        private void BuildCache(IEnumerable<NancyModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleType = module.GetType();
                var moduleKey = _ModuleKeyGenerator.GetKeyForModuleType(moduleType);

                AddMethodRoutesToCache(module, moduleKey, "GET");
                AddMethodRoutesToCache(module, moduleKey, "POST");
                AddMethodRoutesToCache(module, moduleKey, "PUT");
                AddMethodRoutesToCache(module, moduleKey, "DELETE");
            }
        }

        public IEnumerator<RouteCacheEntry> GetEnumerator()
        {
            return _Cache.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Cache.GetEnumerator();
        }

        private void AddMethodRoutesToCache(NancyModule module, string moduleKey, string method)
        {
            foreach (var description in module.GetRouteDescription(method))
            {
                _Cache.Add(new RouteCacheEntry(moduleKey, method, description.Path, description.Condition));
            }
        }
    }
}
