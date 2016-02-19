namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for the razor view engine.
    /// </summary>
    public interface IRazorConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether to automatically include the model's namespace in the generated code.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the model's namespace should be automatically included in the generated code; otherwise, <c>false</c>.
        /// </value>
        bool AutoIncludeModelNamespace { get; }

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
