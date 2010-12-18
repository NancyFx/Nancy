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
        private readonly IModuleActivator activator;

        public AppDomainModuleLocator(IModuleActivator activator)
        {
            this.activator = activator;
        }

        public IEnumerable<NancyModule> GetModules()
        {
            return modules ?? (modules = LocateModulesInAppDomain());
        }

        private IEnumerable<NancyModule> LocateModulesInAppDomain()
        {
            var moduleType = typeof(NancyModule);

            var locatedModules =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetExportedTypes()
                where !type.IsAbstract
                where moduleType.IsAssignableFrom(type)
                where activator.CanCreateInstance(type)
                select activator.CreateInstance(type);

            return locatedModules.ToList();
        }
    }
}