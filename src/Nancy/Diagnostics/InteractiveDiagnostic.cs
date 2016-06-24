namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    /// <summary>
    /// An interactive diagnostic instance.
    /// </summary>
    public class InteractiveDiagnostic
    {
        /// <summary>
        /// Gets or sets the diagnostic name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the diagnostic description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the diagnostic methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        public IEnumerable<InteractiveDiagnosticMethod> Methods { get; set; }
    }
}