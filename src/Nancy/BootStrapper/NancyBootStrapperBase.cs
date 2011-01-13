using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Routing;

namespace Nancy.BootStrapper
{
    /// <summary>
    /// Base class for container based BootStrappers.
    /// 
    /// There are two component lifecycles, an application level one which is guaranteed to be generated at least once (on app startup)
    /// and a request level one which should be guaranteed to be generated per-request. Depending on implementation details the application
    /// lifecycle components may also be generated per request (without any critical issues), but this isn't ideal.
    /// 
    /// Doesn't have to be used (only INancyBootStrapper is required), but does provide a nice consistent base if possible.
    /// 
    /// The methods are called as follows:
    /// 
    /// * Application Level *
    /// CreateContainer() - for creating an empty container
    /// GetModuleTypes() - getting the module types in the application, default implementation grabs from the appdomain
    /// RegisterModules() - register the modules into the container
    /// ConfigureApplicationContainer() - register any application lifecycle dependencies
    /// GetEngineInternal() - construct the container (if required) and resolve INancyEngine
    /// 
    /// * Request Level *
    /// ConfigureRequestContainer() - should be called per-request for registering modules that require request level lifetime.
    /// </summary>
    /// <typeparam name="TContainer">Container tyope</typeparam>
    public abstract class NancyBootStrapperBase<TContainer> : INancyBootStrapper
        where TContainer: class
    {
        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        public INancyEngine GetEngine()
        {
            var container = CreateContainer();
            RegisterModules(GetModuleTypes());
            ConfigureApplicationContainer(container);
            return GetEngineInternal();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected abstract INancyEngine GetEngineInternal();

        /// <summary>
        /// Get the moduleKey generator - defaults to <see cref="DefaultModuleKeyGenerator"/>
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected virtual Nancy.BootStrapper.IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return new Nancy.BootStrapper.DefaultModuleKeyGenerator();
        }

        /// <summary>
        /// Returns available NancyModule types
        /// </summary>
        /// <returns>IEnumerable containing all NancyModule Type definitions</returns>
        protected virtual IEnumerable<ModuleRegistration> GetModuleTypes()
        {
            var moduleType = typeof(NancyModule);
            var moduleKeyGenerator = GetModuleKeyGenerator();

            var locatedModuleTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic
                from type in assembly.GetExportedTypes()
                where !type.IsAbstract
                where moduleType.IsAssignableFrom(type)
                select new ModuleRegistration(type, moduleKeyGenerator.GetKeyForModuleType(type));

            return locatedModuleTypes;
        }

        /// <summary>
        /// Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container</returns>
        protected abstract TContainer CreateContainer();

        /// <summary>
        /// Configure the container (register types) for the application level
        /// 
        /// <seealso cref="ConfigureRequestContainer"/>
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void ConfigureApplicationContainer(TContainer container)
        {
        }

        /// <summary>
        /// Configure the container with per-request registrations
        /// 
        /// Should be called per-request, usually during INancyModuleCatalog.GetModules()
        /// </summary>
        /// <param name="container"></param>
        protected virtual void ConfigureRequestContainer(TContainer container)
        {
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="moduleTypes">NancyModule types</param>
        protected abstract void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes);
    }
}
