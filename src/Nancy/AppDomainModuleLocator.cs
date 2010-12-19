namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    
    /// <summary>
    /// Locates all <see cref="NancyModule"/> instances in the current application domain. The modules
    /// have to be public, have a default ctor and not be abstract.
    /// </summary>
    /// <remarks>This class is a temporary implementation that should be replaced when Nancy supports integration of IoC containers.</remarks>
    public class AppDomainModuleLocator : INancyModuleLocator
    {
        private IDictionary<string, IEnumerable<ModuleMeta>> modules;
        private readonly IModuleActivator activator;
        private static readonly object locker = new object();

        public AppDomainModuleLocator(IModuleActivator activator)
        {

            this.activator = activator;
        }

        //yes, double-if aren't guaranteed..oh well.
        public IDictionary<string, IEnumerable<ModuleMeta>> GetModules()
        {
            if (this.modules == null)
            {
                lock(locker)
                {
                    if (this.modules == null)
                    {
                        this.modules = LocateModulesInAppDomain();
                    }
                }
            }
            return this.modules;
        }

        private IDictionary<string, IEnumerable<ModuleMeta>> LocateModulesInAppDomain()
        {
            var moduleType = typeof(NancyModule);

            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where !type.IsAbstract
                where moduleType.IsAssignableFrom(type)
                where activator.CanCreateInstance(type)
                select type;

            var metas = CreateModuleMetadataDictionary(types);
            StoreModuleMeta(types, metas);

            return metas;
        }

        private static void StoreModuleMeta(IEnumerable<Type> types, Dictionary<string, IEnumerable<ModuleMeta>> metas)
        {
            foreach(var type in types)
            {
                var module = (NancyModule)Activator.CreateInstance(type);
                ((List<ModuleMeta>)metas["GET"]).Add(new ModuleMeta(type, module.GetRouteDescription("GET")));
                ((List<ModuleMeta>)metas["POST"]).Add(new ModuleMeta(type, module.GetRouteDescription("POST")));
                ((List<ModuleMeta>)metas["PUT"]).Add(new ModuleMeta(type, module.GetRouteDescription("PUT")));
                ((List<ModuleMeta>)metas["DELETE"]).Add(new ModuleMeta(type, module.GetRouteDescription("DELETE")));
                
            }
        }

        private static Dictionary<string, IEnumerable<ModuleMeta>> CreateModuleMetadataDictionary(IEnumerable<Type> types)
        {
            return new Dictionary<string, IEnumerable<ModuleMeta>>(StringComparer.CurrentCultureIgnoreCase)
            {
                {"GET", new List<ModuleMeta>(types.Count())},
                {"POST", new List<ModuleMeta>(types.Count())},
                {"PUT", new List<ModuleMeta>(types.Count())},
                {"DELETE", new List<ModuleMeta>(types.Count())},
            };
        }
    }
}
