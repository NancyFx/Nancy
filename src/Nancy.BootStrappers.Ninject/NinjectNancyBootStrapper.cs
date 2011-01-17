#region Using Directives

using System;
using System.Collections.Generic;
using Nancy;
using Nancy.BootStrapper;
using Nancy.Routing;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Extensions.ChildKernel;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using IRequest = Ninject.Activation.IRequest;
using Request = Ninject.Activation.Request;
using Ninject;

#endregion

namespace Nancy.BootStrappers.Ninject
{
    public class NinjectNancyBootStrapper : NancyBootStrapperBase<IKernel>,
                                            INancyBootStrapperPerRequestRegistration<IKernel>,
                                            INancyModuleCatalog
    {
        protected IKernel _Kernel;

        /// <summary>
        ///   Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return _Kernel.Get<INancyEngine>();
        }

        /// <summary>
        ///   Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container</returns>
        protected override IKernel CreateContainer()
        {
            _Kernel = new StandardKernel();
            return _Kernel;
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected sealed override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return _Kernel.Get<IModuleKeyGenerator>();
        }

        /// <summary>
        ///   Register the given module types into the container
        /// </summary>
        /// <param name = "moduleRegistrations">NancyModule types</param>
        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                _Kernel.Bind(typeof(NancyModule))
                        .To(registrationType.ModuleType)
                        .Named(registrationType.ModuleKey);
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected sealed override void RegisterDefaults(IKernel container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.Bind<INancyModuleCatalog>().ToConstant(this);

            foreach (var typeRegistration in typeRegistrations)
            {
                container.Bind(typeRegistration.RegistrationType).To(typeRegistration.ImplementationType).InSingletonScope();
            }
        }

        /// <summary>
        ///   Configure the container with per-request registrations
        /// </summary>
        /// <param name = "container"></param>
        public virtual void ConfigureRequestContainer(IKernel container)
        {
        }

        /// <summary>
        ///   Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public virtual IEnumerable<NancyModule> GetAllModules()
        {
            var child = new ChildKernel(_Kernel);
            ConfigureRequestContainer(child);
            return child.GetAll<NancyModule>();
        }

        /// <summary>
        ///   Retrieves a specific NancyModule implementation based on its key - should be multi-instance and per-request
        /// </summary>
        /// <param name = "moduleKey">Module key</param>
        /// <returns>NancyModule instance</returns>
        public virtual NancyModule GetModuleByKey(string moduleKey)
        {
            var child = new ChildKernel(_Kernel);
            ConfigureRequestContainer(child);
            return child.Get<NancyModule>(moduleKey);
        }
    }
}