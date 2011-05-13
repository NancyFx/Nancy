namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Catalog of <see cref="NancyModule"/> instances.
    /// </summary>
    public interface INancyModuleCatalog
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        IEnumerable<NancyModule> GetAllModules(NancyContext context);

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key - should be per-request lifetime
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        NancyModule GetModuleByKey(string moduleKey, NancyContext context);
    }
}