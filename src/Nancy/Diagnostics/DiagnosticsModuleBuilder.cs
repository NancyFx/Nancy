namespace Nancy.Diagnostics
{
    using Nancy.ModelBinding;
    using Nancy.Routing;

    internal class DiagnosticsModuleBuilder : INancyModuleBuilder
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly ISerializerFactory serializerFactory;
        private readonly IModelBinderLocator modelBinderLocator;

        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IModelBinderLocator modelBinderLocator)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializerFactory = new DiagnosticsSerializerFactory();
            this.modelBinderLocator = modelBinderLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="INancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="INancyModule"/> instance.</returns>
        public INancyModule BuildModule(INancyModule module, NancyContext context)
        {
            module.Context = context;
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, this.serializerFactory);
            module.ModelBinderLocator = this.modelBinderLocator;

            module.After = new AfterPipeline();
            module.Before = new BeforePipeline();
            module.OnError = new ErrorPipeline();

            return module;
        }
    }
}