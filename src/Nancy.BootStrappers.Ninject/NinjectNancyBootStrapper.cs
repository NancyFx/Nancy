namespace Nancy.Bootstrappers.Ninject
{
    using System;
    using System.Collections.Generic;
    using global::Ninject;
    using global::Ninject.Extensions.ChildKernel;
    using Nancy.Bootstrapper;
    using Nancy.Routing;

    public class NinjectNancyBootstrapper : NancyBootstrapperBase<IKernel>,
                                            INancyBootstrapperPerRequestRegistration<IKernel>,
                                            INancyModuleCatalog
    {
        protected IKernel _Kernel;
        private IEnumerable<ModuleRegistration> _ModuleRegistations;

        /// <summary>
        ///   Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return _Kernel.Get<INancyEngine>();
        }

        protected override void RegisterViewEngines(IKernel container, IEnumerable<Type> viewEngineTypes)
        {
            throw new NotImplementedException();
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

        protected override void RegisterViewSourceProviders(IKernel container, IEnumerable<Type> viewSourceProviders)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Register the given module types into the container
        /// </summary>
        /// <param name = "moduleRegistrations">NancyModule types</param>
        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            // To work around the child container limitations we store these now
            // to register into the child container later.
            _ModuleRegistations = moduleRegistrations;

            RegisterModulesInternal(_Kernel, moduleRegistrations);
        }

        /// <summary>
        /// Register modules in the given container.
        /// </summary>
        /// <param name="kernel">Ninject kernel to register into</param>
        /// <param name="moduleRegistrations">Module registrations</param>
        private void RegisterModulesInternal(IKernel kernel, IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                kernel.Bind(typeof(NancyModule))
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

            container.Bind<Func<IRouteCache>>().ToMethod(ctx =>
            {
                Func<IRouteCache> runc =
                        () => ctx.Kernel.Get<IRouteCache>();
                return runc;
            });
        }

        /// <summary>
        ///   Configure the container with per-request registrations
        /// </summary>
        /// <param name = "container"></param>
        public virtual void ConfigureRequestContainer(IKernel container)
        {
        }

        /// <summary>
        /// Create a child kernel - also registers modules in the child kernel to 
        /// work around child container limitations.
        /// </summary>
        /// <returns>ChildKernel</returns>
        private IKernel GetChildKernel()
        {
            var child = new ChildKernel(_Kernel);

            RegisterModulesInternal(child, _ModuleRegistations);

            return child;
        }

        /// <summary>
        ///   Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public virtual IEnumerable<NancyModule> GetAllModules()
        {
            var child = GetChildKernel();
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
            var child = GetChildKernel();
            ConfigureRequestContainer(child);
            return child.Get<NancyModule>(moduleKey);
        }

        //private class RequestKernel : ChildKernel
        //{
        //    public RequestKernel(IResolutionRoot resolutionRoot)
        //        : base(resolutionRoot)
        //    {
        //    }

        //    /// <summary>
        //    ///   Creates a request for the specified service.
        //    /// </summary>
        //    /// <param name = "service">The service that is being requested.</param>
        //    /// <param name = "constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        //    /// <param name = "parameters">The parameters to pass to the resolution.</param>
        //    /// <param name = "isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
        //    /// <param name = "isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
        //    /// <returns>The created request.</returns>
        //    public override Request CreateRequest(Type service,
        //                                            Func<IBindingMetadata, bool> constraint,
        //                                            IEnumerable<IParameter> parameters,
        //                                            bool isOptional,
        //                                            bool isUnique)
        //    {
        //        if (service == null)
        //        {
        //            throw new ArgumentNullException("service");
        //        }
        //        if (parameters == null)
        //        {
        //            throw new ArgumentNullException("parameters");
        //        }

        //        return new Request(service,
        //                            constraint,
        //                            parameters,
        //                            () => StandardScopeCallbacks.Request(null),
        //                            isOptional,
        //                            isUnique);
        //    }
        //}
    }
}