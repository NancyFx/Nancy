namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Locates all <see cref="NancyModule"/> instances in the current application domain. The modules
    /// have to be public, have a default ctor and not be abstract.
    /// </summary>
    /// <remarks>This class is a temporary implementation that should be replaced when Nancy supports integration of IoC containers.</remarks>
    public class AppDomainModuleLocator : INancyModuleLocator
    {
        private static IEnumerable<NancyModule> modules;

        public IEnumerable<NancyModule> GetModules()
        {
            return modules ?? (modules = LocateModulesInAppDomain());
        }

        private static NancyModule CreateModuleInstance(Type type)
        {
            return (NancyModule)Activator.CreateInstance(type);
        }

        private static IEnumerable<NancyModule> LocateModulesInAppDomain()
        {
            var locatedModules =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetExportedTypes()
                where !type.IsAbstract
                where type.IsSubclassOf(typeof(NancyModule))
                where type.GetConstructor(Type.EmptyTypes) != null
                select CreateModuleInstance(type);

            return locatedModules.ToList();
        }
    }
}