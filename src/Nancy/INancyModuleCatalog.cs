namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Catalog of <see cref="NancyModule"/> instances.
    /// </summary>
    public interface INancyModuleCatalog
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
<<<<<<< HEAD
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        IEnumerable<NancyModule> GetAllModules();
=======
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        IEnumerable<NancyModule> GetAllModules(NancyContext context);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key - should be multi-instance and per-request
        /// </summary>
        /// <param name="moduleKey">Module key</param>
<<<<<<< HEAD
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        NancyModule GetModuleByKey(string moduleKey);
=======
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        NancyModule GetModuleByKey(string moduleKey, NancyContext context);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
    }
}