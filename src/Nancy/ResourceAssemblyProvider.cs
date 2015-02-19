namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Default set of assemblies that should be scanned for items (views, text, content etc)
    /// embedded as resources.
    /// </summary>
    /// <remarks>The default convention will scan all assemblies that references another assemblies that has a name that starts with Nancy*</remarks>
    public class ResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private IEnumerable<Assembly> filteredAssemblies;

        /// <summary>
        /// Gets a list of assemblies that should be scanned for views.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = GetFilteredAssemblies()));
        }

        private static IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return AppDomainAssemblyTypeScanner.Assemblies
                .Where(x => !x.IsDynamic)
                .Where(x => !x.GetName().Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase));
        }
    }
}