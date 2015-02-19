namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    using Nancy.ModelBinding;
    using Nancy.Responses;
    using Nancy.Routing;

    internal class DiagnosticsModuleBuilder : INancyModuleBuilder
    {
        private readonly IRootPathProvider rootPathProvider;

        private readonly IEnumerable<ISerializer> serializers;
        private readonly IModelBinderLocator modelBinderLocator;

        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IModelBinderLocator modelBinderLocator)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializers = new[] { new DefaultJsonSerializer { RetainCasing = false } };
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
            // Currently we don't connect view location, binders etc.
            module.Context = context;
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, serializers);
            module.ModelBinderLocator = this.modelBinderLocator;

            module.After = new AfterPipeline();
            module.Before = new BeforePipeline();
            module.OnError = new ErrorPipeline();

            return module;
        }
    }
}