namespace Nancy
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Base class implementation of <see cref="IAssemblyCatalog"/> which adds per-strategy caching.
    /// </summary>
    public abstract class AssemblyCatalogBase : IAssemblyCatalog
    {
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies;
        private readonly ConcurrentDictionary<AssemblyResolveStrategy, IReadOnlyCollection<Assembly>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCatalogBase"/> class.
        /// </summary>
        protected AssemblyCatalogBase()
        {
            this.cache = new ConcurrentDictionary<AssemblyResolveStrategy, IReadOnlyCollection<Assembly>>();
            this.assemblies = new Lazy<IReadOnlyCollection<Assembly>>(this.GetAvailableAssemblies);
        }

        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <param name="strategy">An <see cref="AssemblyResolveStrategy"/> that should be used when resolving assemblies.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies(AssemblyResolveStrategy strategy)
        {
            return this.cache.GetOrAdd(strategy, s => this.assemblies.Value.Where(s.Invoke).ToArray());
        }

        /// <summary>
        /// Get all available <see cref="Assembly"/> instances.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        protected abstract IReadOnlyCollection<Assembly> GetAvailableAssemblies();
    }
}
