namespace Nancy.Diagnostics
{
    using Nancy.Configuration;
    using Nancy.ModelBinding;
    using Nancy.Routing;

    internal class DiagnosticsModuleBuilder : INancyModuleBuilder
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly ISerializerFactory serializerFactory;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly INancyEnvironment environment;

        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IModelBinderLocator modelBinderLocator, INancyEnvironment diagnosticsEnvironment, INancyEnvironment environment)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializerFactory = new DiagnosticsSerializerFactory(diagnosticsEnvironment);
            this.modelBinderLocator = modelBinderLocator;
            this.environment = environment;
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
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, this.serializerFactory, this.environment);
            module.ModelBinderLocator = this.modelBinderLocator;

            module.After = new AfterPipeline();
            module.Before = new BeforePipeline();
            module.OnError = new ErrorPipeline();

            return module;
        }
    }
}