namespace Nancy.Routing
{
    /// <summary>
    /// Defines the functionality to build a fully configured NancyModule instance.
    /// </summary>
    public interface INancyModuleBuilder
    {
        /// <summary>
        /// Builds a fully configured <see cref="INancyModule"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="INancyModule"/> instance.</returns>
        INancyModule BuildModule(INancyModule module, NancyContext context);
    }
}