namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for Nancy interactive diagnostics
    /// </summary>
    public interface IInteractiveDiagnostics
    {
        /// <summary>
        /// Gets the list of available <see cref="InteractiveDiagnostic"/>.
        /// </summary>
        /// <value>The available diagnostics. <seealso cref="InteractiveDiagnostic"/></value>
        IEnumerable<InteractiveDiagnostic> AvailableDiagnostics { get; }

        /// <summary>
        /// Executes the <see cref="InteractiveDiagnosticMethod"/>.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The <see cref="InteractiveDiagnosticMethod"/> instance</param>
        /// <param name="arguments">The <see cref="InteractiveDiagnosticMethod"/> arguments.</param>
        /// <returns>The result of the <see cref="InteractiveDiagnosticMethod"/> as <see cref="object"/></returns>
        object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnosticMethod, object[] arguments);

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The <see cref="InteractiveDiagnosticMethod"/> instance</param>
        /// <returns>The template as <see cref="string"/></returns>
        string GetTemplate(InteractiveDiagnosticMethod interactiveDiagnosticMethod);

        /// <summary>
        /// Gets the <see cref="InteractiveDiagnostic"/>.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>The <see cref="InteractiveDiagnostic"/> instance.</returns>
        InteractiveDiagnostic GetDiagnostic(string providerName);

        /// <summary>
        /// Gets the <see cref="InteractiveDiagnosticMethod"/>.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>The <see cref="InteractiveDiagnosticMethod"/> instance</returns>
        InteractiveDiagnosticMethod GetMethod(string providerName, string methodName);
    }
}