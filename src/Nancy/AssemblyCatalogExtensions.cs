namespace Nancy
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///  Contains extension methods for <see cref="IAssemblyCatalog"/> implementations.
    /// </summary>
    public static class AssemblyCatalogExtensions
    {
        /// <summary>
        /// Gets all <see cref="Assembly"/> instances using the <see cref="AssemblyResolveStrategies.All"/> strategy.
        /// </summary>
        /// <param name="assemblyCatalog">The <see cref="IAssemblyCatalog"/> instance that the assemblies should be reolved from.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public static IReadOnlyCollection<Assembly> GetAssemblies(this IAssemblyCatalog assemblyCatalog)
        {
            return assemblyCatalog.GetAssemblies(AssemblyResolveStrategies.All);
        }
    }
}
