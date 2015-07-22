namespace Nancy.Demo.Razor.Localization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Bootstrapper;

    /// <summary>
    /// Use a custom <see cref="IResourceAssemblyProvider"/> because the default one ignores any
    /// assembly that starts with Nancy*. For normal applications this is not required to
    /// implement.
    /// </summary>
    public class CustomResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private readonly IAssemblyCatalog assemblyCatalog;

        public CustomResourceAssemblyProvider(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
        }

        private IEnumerable<Assembly> filteredAssemblies;

        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = this.GetFilteredAssemblies()));
        }

        private IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return this.assemblyCatalog.Assemblies.Where(x => !x.IsDynamic);
        }
    }
}