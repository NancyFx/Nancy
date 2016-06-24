namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for interactive diagnostics.
    /// </summary>
    public interface IInteractiveDiagnostics
    {
        /// <summary>
        /// Gets the list of available diagnostics.
        /// </summary>
        /// <value>
        /// The available diagnostics.
        /// </value>
        IEnumerable<InteractiveDiagnostic> AvailableDiagnostics { get; }

        /// <summary>
        /// Executes the diagnostic.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The interactive diagnostic method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnosticMethod, object[] arguments);

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The interactive diagnostic method.</param>
        /// <returns></returns>
        string GetTemplate(InteractiveDiagnosticMethod interactiveDiagnosticMethod);

        /// <summary>
        /// Gets the diagnostic.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        InteractiveDiagnostic GetDiagnostic(string providerName);

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        InteractiveDiagnosticMethod GetMethod(string providerName, string methodName);
    }
}