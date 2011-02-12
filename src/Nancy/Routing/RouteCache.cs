namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;

    public class RouteCache : Dictionary<string, List<RouteDescription>>
    {
        private readonly IModuleKeyGenerator moduleKeyGenerator;

        public RouteCache(INancyModuleCatalog moduleCatalog, IModuleKeyGenerator moduleKeyGenerator)
        {
            this.moduleKeyGenerator = moduleKeyGenerator;

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

        private void AddMethodRoutesToCache(NancyModule module, string moduleKey, string method)
        {
            var routes = module.GetRoutes(method);
            
            if (!this.ContainsKey(moduleKey))
            {
                this[moduleKey] = new List<RouteDescription>();
            }

            this[moduleKey].AddRange(routes.Select(r => r.Description));
        }

        public bool IsEmpty()
        {
            return this.Values.SelectMany(r => r).Count() == 0;
        }
    }
}
