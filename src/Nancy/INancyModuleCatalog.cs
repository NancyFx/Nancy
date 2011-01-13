namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Extensions;
    using Nancy.Routing;

    /// <summary>
    /// Catalog of NancyModule instances
    /// </summary>
    public interface INancyModuleCatalog
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        IEnumerable<NancyModule> GetAllModules();

        /// <summary>
        /// Retrieves a specific NancyModule implementation based on its key - should be multi-instance and per-request
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <returns>NancyModule instance</returns>
        NancyModule GetModuleByKey(string moduleKey);
    }
}
