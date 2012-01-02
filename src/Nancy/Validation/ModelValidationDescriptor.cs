namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A description of the rules a validator provides.
    /// </summary>
    public class ModelValidationDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationDescriptor"/> class.
        /// </summary>
        /// <param name="rules">The rules.</param>
        public ModelValidationDescriptor(IEnumerable<ModelValidationRule> rules)
        {
            this.Rules = rules == null
                ? new List<ModelValidationRule>().AsReadOnly()
                : rules.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</value>
        public IEnumerable<ModelValidationRule> Rules { get; private set; }
    }
}