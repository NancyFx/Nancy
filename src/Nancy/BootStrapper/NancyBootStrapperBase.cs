namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Nancy.Diagnostics;
    using Nancy.Routing;
    using Nancy.Extensions;
    using ViewEngines;

    /// <summary>
    /// Base class for container based Bootstrappers.
    /// 
    /// There are two component lifecycles, an application level one which is guaranteed to be generated at least once (on app startup)
    /// and a request level one which should be guaranteed to be generated per-request. Depending on implementation details the application
    /// lifecycle components may also be generated per request (without any critical issues), but this isn't ideal.
    /// 
    /// Doesn't have to be used (only INancyBootstrapper is required), but does provide a nice consistent base if possible.
    /// 
    /// The methods in the base class are all Application level are called as follows:
    /// 
    /// CreateContainer() - for creating an empty container
    /// GetModuleTypes() - getting the module types in the application, default implementation grabs from the appdomain
    /// RegisterModules() - register the modules into the container
    /// ConfigureApplicationContainer() - register any application lifecycle dependencies
    /// GetEngineInternal() - construct the container (if required) and resolve INancyEngine
    /// 
    /// Request level implementations may use <see cref="INancyBootstrapperPerRequestRegistration{TContainer}"/>, or implement custom
    /// lifetime logic. It is preferred that users have the ability to register per-request scoped dependencies, and that instances retrieved
    /// via <see cref="INancyModuleCatalog.GetModuleByKey"/> are per-request scoped.
    /// </summary>
    /// <typeparam name="TContainer">Container tyope</typeparam>
    public abstract class NancyBootstrapperBase<TContainer> : INancyBootstrapper
        where TContainer : class
    {
        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteResolver { get { return typeof(DefaultRouteResolver); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRoutePatternMatcher { get { return typeof (DefaultRoutePatternMatcher); } }
        
        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultTemplateEngineSelector { get { return typeof(DefaultTemplateEngineSelector); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultNancyEngine { get { return typeof(NancyEngine); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultModuleKeyGenerator { get { return typeof(DefaultModuleKeyGenerator); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteCache { get { return typeof(RouteCache); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteCacheProvider { get { return typeof(DefaultRouteCacheProvider); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultViewLocator { get { return typeof (AspNetTemplateLocator); } }

        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        public INancyEngine GetEngine()
        {
            var container = CreateContainer();
            ConfigureApplicationContainer(container);
            
            RegisterDefaults(container, BuildDefaults());
            RegisterModules(GetModuleTypes(GetModuleKeyGenerator()));
            RegisterViewEngines(container, GetViewEngineTypes());

            return GetEngineInternal();
        }

        private IEnumerable<TypeRegistration> BuildDefaults()
        {
            return new[]
            {
                new TypeRegistration(typeof(IRouteResolver), DefaultRouteResolver),
                new TypeRegistration(typeof(ITemplateEngineSelector), DefaultTemplateEngineSelector),
                new TypeRegistration(typeof(INancyEngine), DefaultNancyEngine),
                new TypeRegistration(typeof(IModuleKeyGenerator), DefaultModuleKeyGenerator),
                new TypeRegistration(typeof(IRouteCache), DefaultRouteCache),
                new TypeRegistration(typeof(IRouteCacheProvider), DefaultRouteCacheProvider),
                new TypeRegistration(typeof(IRoutePatternMatcher), DefaultRoutePatternMatcher),
                new TypeRegistration(typeof(IViewLocator), DefaultViewLocator), 
            };
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected abstract INancyEngine GetEngineInternal();

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected abstract IModuleKeyGenerator GetModuleKeyGenerator();

        /// <summary>
        /// Returns available NancyModule types
        /// </summary>
        /// <returns>IEnumerable containing all NancyModule Type definitions</returns>
        protected virtual IEnumerable<ModuleRegistration> GetModuleTypes(IModuleKeyGenerator moduleKeyGenerator)
        {
            var moduleType = typeof(NancyModule);

            var locatedModuleTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.ReflectionOnly
                where !assembly.IsDynamic
                from type in assembly.SafeGetExportedTypes()
                where !type.IsAbstract
                where moduleType.IsAssignableFrom(type)
                select new ModuleRegistration(type, moduleKeyGenerator.GetKeyForModuleType(type));

            return locatedModuleTypes;
        }

        protected virtual IEnumerable<Type> GetViewEngineTypes()
        {
            //var viewEngineTypes =
            //    from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //    where !assembly.ReflectionOnly
            //    where !assembly.IsDynamic
            //    from type in assembly.SafeGetExportedTypes()
            //    where !type.IsAbstract
            //    where typeof(IFooBar).IsAssignableFrom(type)
            //    select type;

            //return viewEngineTypes;
            return Enumerable.Empty<Type>();
        }

        protected abstract void RegisterViewEngines(TContainer container, IEnumerable<Type> viewEngineTypes);

        /// <summary>
        /// Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container</returns>
        protected abstract TContainer CreateContainer();

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected abstract void RegisterDefaults(TContainer container, IEnumerable<TypeRegistration> typeRegistrations);

        /// <summary>
        /// Configure the container (register types) for the application level
        /// <seealso cref="ConfigureRequestContainer"/>
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void ConfigureApplicationContainer(TContainer container)
        {
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected abstract void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes);
    }
}
