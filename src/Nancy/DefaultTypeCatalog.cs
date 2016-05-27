namespace Nancy
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Nancy.Extensions;

    /// <summary>
    /// Default implementation of the <see cref="ITypeCatalog"/> interface.
    /// </summary>
    public class DefaultTypeCatalog : ITypeCatalog
    {
        private readonly IAssemblyCatalog assemblyCatalog;
        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<Type>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeCatalog"/> class.
        /// </summary>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instanced, used to get the assemblies that types should be resolved from.</param>
        public DefaultTypeCatalog(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
            this.cache = new ConcurrentDictionary<Type, IReadOnlyCollection<Type>>();
        }

        /// <summary>
        /// Gets all types that are assignable to the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that returned types should be assignable to.</param>
        /// <param name="strategy">A <see cref="TypeResolveStrategy"/> that should be used when retrieving types.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        public IReadOnlyCollection<Type> GetTypesAssignableTo(Type type, TypeResolveStrategy strategy)
        {
            return this.cache.GetOrAdd(type, t => this.GetTypesAssignableTo(type)).Where(strategy.Invoke).ToArray();
        }

        private IReadOnlyCollection<Type> GetTypesAssignableTo(Type type)
        {
            return this.assemblyCatalog
                .GetAssemblies()
                .SelectMany(assembly => assembly.SafeGetExportedTypes())
                .Where(type.IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract)
                .ToArray();
        }
    }
}
