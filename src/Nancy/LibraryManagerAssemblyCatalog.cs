#if DNX
namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.PlatformAbstractions;

    /// <summary>
    /// Default implementation of the <see cref="IAssemblyCatalog"/> interface, based on
    /// retrieving <see cref="Assembly"/> information from an <see cref="ILibraryManager"/>.
    /// </summary>
    public class LibraryManagerAssemblyCatalog : IAssemblyCatalog
    {
        private readonly ILibraryManager libraryManager;
        private readonly AssemblyName nancyAssemblyName = typeof(LibraryManagerAssemblyCatalog).GetTypeInfo().Assembly.GetName();
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryManagerAssemblyCatalog"/> class.
        /// </summary>
        public LibraryManagerAssemblyCatalog()
        {
            this.libraryManager = PlatformServices.Default.LibraryManager;
            this.assemblies = new Lazy<IReadOnlyCollection<Assembly>>(this.LoadAllNancyReferencingAssemblies);
        }

        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies()
        {
            return this.assemblies.Value;
        }

        public IReadOnlyCollection<Assembly> LoadAllNancyReferencingAssemblies()
        {
            var results = new HashSet<Assembly>
            {
                typeof (INancyEngine).GetTypeInfo().Assembly
            };

            var referencingLibraries = this.libraryManager.GetReferencingLibraries(this.nancyAssemblyName.Name);

            foreach (var assemblyName in referencingLibraries.SelectMany(referencingLibrary => referencingLibrary.Assemblies))
            {
                results.Add(SafeLoadAssembly(assemblyName));
            }

            return results.ToArray();
        }

        private static Assembly SafeLoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
#endif
