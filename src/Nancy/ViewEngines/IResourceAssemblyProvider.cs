namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality for retrieving which assemblies that should have their resources scanned for views.
    /// </summary>
    public interface IResourceAssemblyProvider
    {
        /// <summary>
        /// Gets a list of assemblies that should be scanned for views embedded as resources.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        IEnumerable<Assembly> GetAssembliesToScan();
    }
}