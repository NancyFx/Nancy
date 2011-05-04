namespace Nancy
{
    using System;
    using System.Collections.Generic;

    using Bootstrapper;

    using TinyIoC;

    /// <summary>
    /// TinyIoC bootstrapper - registers default route resolver and registers itself as
    /// INancyModuleCatalog for resolving modules but behaviour can be overridden if required.
    /// </summary>
    public class DefaultNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<TinyIoCContainer>
    {
        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.AutoRegister();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override sealed INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.Resolve<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected override sealed IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.ApplicationContainer.Resolve<IModuleKeyGenerator>();
        }

        /// <summary>
        /// Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container instance</returns>
        protected override TinyIoCContainer GetApplicationContainer()
        {
            return new TinyIoCContainer();
        }

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override sealed void RegisterBootstrapperTypes(TinyIoCContainer applicationContainer)
        {
            applicationContainer.Register<INancyModuleCatalog>(this);
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected override sealed void RegisterTypes(TinyIoCContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                container.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsSingleton();
            }
        }

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="collectionTypeRegistrationsn">Collection type registrations to register</param>
        protected override sealed void RegisterCollectionTypes(TinyIoCContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
            {
                container.RegisterMultiple(collectionTypeRegistration.RegistrationType, collectionTypeRegistration.ImplementationTypes);
            }
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override sealed void RegisterRequestContainerModules(TinyIoCContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var moduleRegistrationType in moduleRegistrationTypes)
            {
                container.Register(
                    typeof(NancyModule), 
                    moduleRegistrationType.ModuleType, 
                    moduleRegistrationType.ModuleKey).
                    AsSingleton();
            }
        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <returns>Request container instance</returns>
        protected override sealed TinyIoCContainer CreateRequestContainer()
        {
            return this.ApplicationContainer.GetChildContainer();
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of NancyModule instances</returns>
        protected override sealed IEnumerable<NancyModule> GetAllModules(TinyIoCContainer container)
        {
            return container.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Retreive a specific module instance from the container by its key
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleKey">Module key of the module</param>
        /// <returns>NancyModule instance</returns>
        protected override sealed NancyModule GetModuleByKey(TinyIoCContainer container, string moduleKey)
        {
            return container.Resolve<NancyModule>(moduleKey);
        }
    }
}