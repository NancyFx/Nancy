namespace Nancy
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality for retrieving which assemblies that should be used by Nancy.
    /// </summary>
    public interface IResourceAssemblyProvider
    {
        /// <summary>
        /// Gets a list of assemblies that should be scanned.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        IEnumerable<Assembly> GetAssembliesToScan();
    }
}