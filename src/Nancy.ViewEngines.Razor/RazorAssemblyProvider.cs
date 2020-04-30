namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides access to the <see cref="Assembly"/> that are available to Razor.
    /// </summary>
    public class RazorAssemblyProvider
    {
        private readonly IRazorConfiguration configuration;
        private readonly IAssemblyCatalog assemblyCatalog;
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies;
        private readonly string[] defaultAssemblyDefinitions = {
            "mscorlib",
            "Microsoft.CSharp",
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.Core",
            "System.Runtime"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorAssemblyProvider"/> class.
        /// </summary>
        /// <param name="configuration">An <see cref="IRazorConfiguration"/> instance.</param>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instance.</param>
        public RazorAssemblyProvider(IRazorConfiguration configuration, IAssemblyCatalog assemblyCatalog)
        {
            this.configuration = configuration;
            this.assemblyCatalog = assemblyCatalog;
            this.assemblies = new Lazy<IReadOnlyCollection<Assembly>>(this.GetAllAssemblies);
        }

        /// <summary>
        /// Get all <see cref="Assembly"/> instances that are available to the Razor engine.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies()
        {
            return this.assemblies.Value;
        }

        private IReadOnlyCollection<Assembly> GetAllAssemblies()
        {
            return this.assemblyCatalog.GetAssemblies()
                       .Union(this.LoadAssembliesInConfiguration())
                       .Union(this.GetDefaultAssemblies())
                       .ToArray();
        }

        private IEnumerable<Assembly> LoadAssembliesInConfiguration()
        {
            var loadedAssemblies = new HashSet<Assembly>();

            var validAssemblyDefinitions = this.configuration
                .GetAssemblyNames()
                .Where(definition => !string.IsNullOrEmpty(definition));

            foreach (var assemblyDefinition in validAssemblyDefinitions)
            {
                try
                {
                    loadedAssemblies.Add(Assembly.Load(assemblyDefinition));
                }
                catch
                {
                }
            }

            return loadedAssemblies;
        }

        private IEnumerable<Assembly> GetDefaultAssemblies()
        {
            var loadedAssemblies = new HashSet<Assembly>();

            foreach (var assemblyDefinition in this.defaultAssemblyDefinitions)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().OrderByDescending(x => x.GetName().Version).FirstOrDefault(x => x.GetName().Name.Equals(assemblyDefinition, StringComparison.OrdinalIgnoreCase));

                if (assembly != null)
                {
                    loadedAssemblies.Add(assembly);
                }
            }

            return loadedAssemblies;
        }
    }
}
