namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using FluentValidation.Internal;

    /// <summary>
    /// Defines the core functionality of an adapter between Fluent Validation validators and Nancy validation rules.
    /// </summary>
    /// <typeparam name="T">The type of the Fluent Validation validator that is being mapped. Has to inherit from <see cref="IPropertyValidator"/>.</typeparam>
    public abstract class AdapterBase<T> : IFluentAdapter where T : IPropertyValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterBase{T}"/> class for the specified
        /// <paramref name="rule"/> and <paramref name="validator"/>.
        /// </summary>
        /// <param name="rule">The fluent validation <see cref="PropertyRule"/> that is being mapped.</param>
        /// <param name="validator">The <see cref="IPropertyValidator"/> of the rule.</param>
        protected AdapterBase(PropertyRule rule, T validator)
        {
            this.Rule = rule;
            this.Validator = validator;
        }

        /// <summary>
        /// Gets the he fluent validation <see cref="PropertyRule"/> that is being mapped.
        /// </summary>
        /// <value>A <see cref="PropertyRule"/> instance.</value>
        public PropertyRule Rule { get; private set; }

        /// <summary>
        /// Gets the <see cref="IPropertyValidator"/> of the rule.
        /// </summary>
        /// <value>An <see cref="IPropertyValidator"/> instance.</value>
        public T Validator { get; set; }

        /// <summary>
        /// Get the <see cref="ValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationRule"/> instances.</returns>
        public abstract IEnumerable<ValidationRule> GetRules();

        /// <summary>
        /// Gets the name of the members that the validator applied to.
        /// </summary>
        /// <returns>A string containing the name of members that the validator is applied to.</returns>
        protected virtual IEnumerable<string> GetMemberNames()
        {
            yield return this.Rule.PropertyName;
        }

        /// <summary>
        /// Get the formatted error message of the validator.
        /// </summary>
        /// <param name="displayName">The name to show for the member.</param>
        /// <returns>A formatted error message string.</returns>
        protected virtual string FormatMessage(string displayName)
        {
            return new MessageFormatter()
                .AppendPropertyName(displayName ?? this.Rule.GetDisplayName())
                .BuildMessage(this.Validator.ErrorMessageSource.GetString());
        }
    }
}