namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A description of the rules a validator provides.
    /// </summary>
    public class ValidationDescriptor
    {
        /// <summary>
        /// Gets the rules.
        /// </summary>
        public IEnumerable<ValidationRule> Rules { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationDescriptor"/> class.
        /// </summary>
        /// <param name="rules">The rules.</param>
        public ValidationDescriptor(IEnumerable<ValidationRule> rules)
        {
            Rules = rules == null
                ? new List<ValidationRule>().AsReadOnly()
                : rules.ToList().AsReadOnly();
        }
    }
}