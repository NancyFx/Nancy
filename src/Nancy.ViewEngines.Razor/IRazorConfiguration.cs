namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for the razor view engine.
    /// </summary>
    public interface IRazorConfiguration
    {
        /// <summary>
        /// Gets the assembly names.
        /// </summary>
        IEnumerable<string> GetAssemblyNames();

        /// <summary>
        /// Gets the default namespaces.
        /// </summary>
        IEnumerable<string> GetDefaultNamespaces();
    }
}
