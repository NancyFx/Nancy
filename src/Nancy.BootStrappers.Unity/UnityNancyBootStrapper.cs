using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Nancy.Bootstrapper;
using Nancy.ViewEngines;

namespace Nancy.Bootstrappers.Unity
{
    using System;

    public class UnityNancyBootstrapper : NancyBootstrapperBase<IUnityContainer>,
                                          INancyBootstrapperPerRequestRegistration<IUnityContainer>,
                                          INancyModuleCatalog
    {
        protected IUnityContainer _UnityContainer;

        // We override this with a wrapper to work around the lack of IEnumerable<T> dependency support
        protected override System.Type DefaultTemplateEngineSelector { get { return typeof(UnityTemplateEngineSelector); } }

        /// <summary>
        ///   Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return _UnityContainer.Resolve<INancyEngine>();
        }

        protected override void RegisterViewEngines(IUnityContainer container, IEnumerable<Type> viewEngineTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container</returns>
        protected override IUnityContainer CreateContainer()
        {
            _UnityContainer = new UnityContainer();
            return _UnityContainer;
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected sealed override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return _UnityContainer.Resolve<IModuleKeyGenerator>();
        }

        protected override void RegisterViewSourceProviders(IUnityContainer container, IEnumerable<Type> viewSourceProviders)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Register the given module types into the container
        /// </summary>
        /// <param name = "moduleRegistrations">NancyModule types</param>
        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                _UnityContainer.RegisterType(
                    typeof(NancyModule),
                    registrationType.ModuleType,
                    registrationType.ModuleKey);
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected sealed override void RegisterDefaults(IUnityContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.RegisterInstance<INancyModuleCatalog>(this);

            foreach (var typeRegistration in typeRegistrations)
            {
                container.RegisterType(
                    typeRegistration.RegistrationType,
                    typeRegistration.ImplementationType,
                    new ContainerControlledLifetimeManager());
            }
        }

        /// <summary>
        ///   Configure the container with per-request registrations
        /// </summary>
        /// <param name = "container"></param>
        public virtual void ConfigureRequestContainer(IUnityContainer container)
        {
        }

        /// <summary>
        ///   Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public virtual IEnumerable<NancyModule> GetAllModules()
        {
            var child = _UnityContainer.CreateChildContainer();
            ConfigureRequestContainer(child);
            return child.ResolveAll<NancyModule>();
        }

        /// <summary>
        ///   Retrieves a specific NancyModule implementation based on its key - should be multi-instance and per-request
        /// </summary>
        /// <param name = "moduleKey">Module key</param>
        /// <returns>NancyModule instance</returns>
        public virtual NancyModule GetModuleByKey(string moduleKey)
        {
            var child = _UnityContainer.CreateChildContainer();
            ConfigureRequestContainer(child);
            return child.Resolve<NancyModule>(moduleKey);
        }
    }
}