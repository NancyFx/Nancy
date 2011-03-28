namespace Nancy.Routing
{
    using ModelBinding;
    using Nancy.ViewEngines;

    /// <summary>
    /// Default implementation for building a full configured <see cref="NancyModule"/> instance.
    /// </summary>
    public class DefaultNancyModuleBuilder : INancyModuleBuilder
    {
        private readonly IViewFactory viewFactory;
        private readonly IResponseFormatter responseFormatter;
        private readonly IModelBinderLocator modelBinderLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNancyModuleBuilder"/> class.
        /// </summary>
        /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be assigned to the module.</param>
        /// <param name="responseFormatter">An <see cref="DefaultResponseFormatter"/> instance that should be assigned to the module.</param>
        /// <param name="modelBinderLocator">A <see cref="IModelBinderLocator"/> instance that should be assigned to the module.</param>
        public DefaultNancyModuleBuilder(IViewFactory viewFactory, IResponseFormatter responseFormatter, IModelBinderLocator modelBinderLocator)
        {
            this.viewFactory = viewFactory;
            this.responseFormatter = responseFormatter;
            this.modelBinderLocator = modelBinderLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="NancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="NancyModule"/> that shoule be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="NancyModule"/> instance.</returns>
        public NancyModule BuildModule(NancyModule module, NancyContext context)
        {
            module.Context = context;
            module.Response = this.responseFormatter;
            module.ViewFactory = this.viewFactory;
            module.ModelBinderLocator = this.modelBinderLocator;

            return module;
        }
    }
}