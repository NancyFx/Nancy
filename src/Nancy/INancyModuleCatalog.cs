namespace Nancy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Catalog of <see cref="INancyModule"/> instances.
    /// </summary>
    public interface INancyModuleCatalog
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        IEnumerable<INancyModule> GetAllModules(NancyContext context);

        /// <summary>
        /// Retrieves a specific <see cref="INancyModule"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="INancyModule"/> instance</returns>
        INancyModule GetModule(Type moduleType, NancyContext context);
    }
}