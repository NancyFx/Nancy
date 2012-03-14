namespace Nancy.Diagnostics
{
    /// <summary>
    /// Defines the functionality a diagnostics provider.
    /// </summary>
    public interface IDiagnosticsProvider
    {
        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the provider.</value>
        string Name { get; }

        /// <summary>
        /// Gets the description of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the description of the provider.</value>
        string Description { get; }

        /// <summary>
        /// Gets the object that contains the interactive diagnostics methods.
        /// </summary>
        /// <value>An instance of the interactive diagnostics object.</value>
        object DiagnosticObject { get; }
    }
}