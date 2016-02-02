namespace Nancy.Demo.Razor.Localization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Use a custom <see cref="IResourceAssemblyProvider"/> because the default one ignores any
    /// assembly that starts with Nancy*. For normal applications this is not required to
    /// implement.
    /// </summary>
    public class CustomResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private IEnumerable<Assembly> filteredAssemblies;

        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = GetFilteredAssemblies()));
        }

        private static IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return AppDomainAssemblyTypeScanner.Assemblies.Where(x => !x.IsDynamic);
        }
    }
}