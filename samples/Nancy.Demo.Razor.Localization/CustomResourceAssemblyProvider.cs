namespace Nancy.Demo.Razor.Localization
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Use a custom <see cref="IResourceAssemblyProvider"/> because the default one ignores any
    /// assembly that starts with Nancy*. For normal applications this is not required to
    /// implement.
    /// </summary>
    public class CustomResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private readonly IAssemblyCatalog assemblyCatalog;
        private IEnumerable<Assembly> filteredAssemblies;

        public CustomResourceAssemblyProvider(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
        }

        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = this.assemblyCatalog.GetAssemblies()));
        }
    }
}