using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Routing;

namespace Nancy.BootStrapper
{
    /// <summary>
    /// Base class for container based BootStrappers
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
            RegisterModules(GetModulesTypes());
            ConfigureApplicationContainer(container);
            return GetEngineInternal();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected abstract INancyEngine GetEngineInternal();

        /// <summary>
        /// Returns available NancyModule types
        /// </summary>
        /// <returns>IEnumerable containing all NancyModule Type definitions</returns>
        protected virtual IEnumerable<Type> GetModulesTypes()
        {
            var moduleType = typeof(NancyModule);

            var locatedModuleTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic
                from type in assembly.GetExportedTypes()
                where !type.IsAbstract
                where moduleType.IsAssignableFrom(type)
                select type;

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
        /// </summary>
        /// <param name="container"></param>
        protected virtual void ConfigureRequestContainer(TContainer container)
        {
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="moduleTypes">NancyModule types</param>
        protected abstract void RegisterModules(IEnumerable<Type> moduleTypes);
    }
}
