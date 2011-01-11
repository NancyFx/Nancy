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
        IEnumerable<NancyModule> GetModules();
    }
}
