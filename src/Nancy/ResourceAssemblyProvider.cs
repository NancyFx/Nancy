namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default set of assemblies that should be scanned for items (views, text, content etc)
    /// embedded as resources.
    /// </summary>
    /// <remarks>The default convention will scan all assemblies that references another assemblies that has a name that starts with Nancy*</remarks>
    public class ResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private readonly IAssemblyCatalog assemblyCatalog;
        private IEnumerable<Assembly> filteredAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAssemblyProvider"/>
        /// </summary>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instance.</param>
        public ResourceAssemblyProvider(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
        }

        /// <summary>
        /// Gets a list of assemblies that should be scanned for views.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = this.GetFilteredAssemblies()));
        }

        private IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return this.assemblyCatalog
                .GetAssemblies()
                .Where(x => !x.GetName().Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase));
        }
    }
}
