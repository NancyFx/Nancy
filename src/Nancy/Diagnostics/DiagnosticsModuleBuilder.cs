namespace Nancy.Diagnostics
{
    using System.Collections.Generic;
    using ModelBinding;
    using Nancy.Routing;

    internal class DiagnosticsModuleBuilder : INancyModuleBuilder
    {
        private readonly IRootPathProvider rootPathProvider;

        private readonly IEnumerable<ISerializer> serializers;
        private readonly IModelBinderLocator modelBinderLocator;

        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IModelBinderLocator modelBinderLocator)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers;
            this.modelBinderLocator = modelBinderLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="NancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="NancyModule"/> that shoule be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="NancyModule"/> instance.</returns>
        public NancyModuleBase BuildModule(NancyModuleBase module, NancyContext context)
        {
            // Currently we don't connect view location, binders etc.
            module.Context = context;
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, serializers);
            module.ModelBinderLocator = this.modelBinderLocator;

            return module;
        }
    }
}