namespace Nancy
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of an assembly catalog.
    /// </summary>
    public interface IAssemblyCatalog
    {
        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <param name="strategy">An <see cref="AssemblyResolveStrategy"/> that should be used when resolving assemblies.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        IReadOnlyCollection<Assembly> GetAssemblies(AssemblyResolveStrategy strategy);
    }
}
