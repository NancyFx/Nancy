namespace Nancy.Routing
{
    using ModelBinding;
    using Nancy.Extensions;
    using Nancy.Responses.Negotiation;
    using Nancy.ViewEngines;
    using Nancy.Validation;

    /// <summary>
    /// Default implementation for building a full configured <see cref="INancyModule"/> instance.
    /// </summary>
    public class DefaultNancyModuleBuilder : INancyModuleBuilder
    {
        private readonly IViewFactory viewFactory;
        private readonly IResponseFormatterFactory responseFormatterFactory;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IModelValidatorLocator validatorLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNancyModuleBuilder"/> class.
        /// </summary>
        /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be assigned to the module.</param>
        /// <param name="responseFormatterFactory">An <see cref="IResponseFormatterFactory"/> instance that should be used to create a response formatter for the module.</param>
        /// <param name="modelBinderLocator">A <see cref="IModelBinderLocator"/> instance that should be assigned to the module.</param>
        /// <param name="validatorLocator">A <see cref="IModelValidatorLocator"/> instance that should be assigned to the module.</param>
        public DefaultNancyModuleBuilder(IViewFactory viewFactory, IResponseFormatterFactory responseFormatterFactory, IModelBinderLocator modelBinderLocator, IModelValidatorLocator validatorLocator)
        {
            this.viewFactory = viewFactory;
            this.responseFormatterFactory = responseFormatterFactory;
            this.modelBinderLocator = modelBinderLocator;
            this.validatorLocator = validatorLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="INancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that shoule be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="INancyModule"/> instance.</returns>
        public INancyModule BuildModule(INancyModule module, NancyContext context)
        {
            CreateNegotiationContext(module, context);

            module.Context = context;
            module.Response = this.responseFormatterFactory.Create(context);
            module.ViewFactory = this.viewFactory;
            module.ModelBinderLocator = this.modelBinderLocator;
            module.ValidatorLocator = this.validatorLocator;

            return module;
        }

        private static void CreateNegotiationContext(INancyModule module, NancyContext context)
        {
            // TODO - not sure if this should be here or not, but it'll do for now :)
            context.NegotiationContext = new NegotiationContext
                                             {
                                                 ModuleName = module.GetModuleName(),
                                                 ModulePath = module.ModulePath,
                                             };
        }
    }
}