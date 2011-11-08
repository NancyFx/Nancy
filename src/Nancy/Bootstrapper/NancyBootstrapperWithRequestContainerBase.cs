namespace Nancy.Bootstrapper
{
    using System.Collections.Generic;

    /// <summary>
    /// Nancy bootstrapper base with per-request container support.
    /// Stores/retrieves the child container in the context to ensure that
    /// only one child container is stored per request, and that the child 
    /// container will be disposed at the end of the request.
    /// </summary>
    /// <typeparam name="TContainer">IoC container type</typeparam>
    public abstract class NancyBootstrapperWithRequestContainerBase<TContainer> : NancyBootstrapperBase<TContainer>
        where TContainer : class
    {
        /// <summary>
        /// Context key for storing the child container in the context
        /// </summary>
        private readonly string contextKey = typeof(TContainer).FullName + "BootstrapperChildContainer";

        /// <summary>
        /// Stores the module registrations to be registered into the request container
        /// </summary>
        private IEnumerable<ModuleRegistration> moduleRegistrationTypeCache;

        /// <summary>
        /// Gets the context key for storing the child container in the context
        /// </summary>
        protected virtual string ContextKey
        {
            get
            {
                return this.contextKey;
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public override sealed IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            var requestContainer = this.GetConfiguredRequestContainer(context);

            return this.GetAllModules(requestContainer);
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public override sealed NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            var requestContainer = this.GetConfiguredRequestContainer(context);

            return this.GetModuleByKey(requestContainer, moduleKey);
        }

        /// <summary>
        /// Creates and initializes the request pipelines.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> used by the request.</param>
        /// <returns>An <see cref="IPipelines"/> instance.</returns>
        protected override sealed IPipelines InitializeRequestPipelines(NancyContext context)
        {
            var requestContainer = 
                this.GetConfiguredRequestContainer(context);

            var requestPipelines =
                new Pipelines(this.ApplicationPipelines);
            
            this.RequestStartup(requestContainer, requestPipelines, context);

            return requestPipelines;
        }

        /// <summary>
        /// Gets the per-request container
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Request container instance</returns>
        protected TContainer GetConfiguredRequestContainer(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue(this.ContextKey, out contextObject);
            var requestContainer = contextObject as TContainer;

            if (requestContainer == null)
            {
                requestContainer = this.CreateRequestContainer();

                this.RegisterRequestContainerModules(requestContainer, this.moduleRegistrationTypeCache);

                context.Items[this.ContextKey] = requestContainer;

                this.ConfigureRequestContainer(requestContainer, context);
            }

            return requestContainer;
        }

        /// <summary>
        /// Configure the request container
        /// </summary>
        /// <param name="container">Request container instance</param>
        /// <param name="context"></param>
        protected virtual void ConfigureRequestContainer(TContainer container, NancyContext context)
        {
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override sealed void RegisterModules(TContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            this.moduleRegistrationTypeCache = moduleRegistrationTypes;
        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <returns>Request container instance</returns>
        protected abstract TContainer CreateRequestContainer();

        /// <summary>
        /// Register the given module types into the request container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected abstract void RegisterRequestContainerModules(TContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes);

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of NancyModule instances</returns>
        protected abstract IEnumerable<NancyModule> GetAllModules(TContainer container);

        /// <summary>
        /// Retreive a specific module instance from the container by its key
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleKey">Module key of the module</param>
        /// <returns>NancyModule instance</returns>
        protected abstract NancyModule GetModuleByKey(TContainer container, string moduleKey);
    }
}