namespace Nancy.Diagnostics
{
    using Nancy.Routing;

    internal class DiagnosticsModuleBuilder : INancyModuleBuilder
    {
        /// <summary>
        /// Builds a fully configured <see cref="NancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="NancyModule"/> that shoule be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="NancyModule"/> instance.</returns>
        public NancyModule BuildModule(NancyModule module, NancyContext context)
        {
            // Currently we don't connect view location, binders etc.
            module.Context = context;

            return module;
        }
    }
}