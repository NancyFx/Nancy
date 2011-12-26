namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using FluentValidation.Internal;

    /// <summary>
    /// Default implementation of a <see cref="AdapterBase{T}"/>.
    /// </summary>
    public class FluentAdapter : AdapterBase<IPropertyValidator>
    {
        private readonly string ruleType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">The name of the rule.</param>
        /// <param name="rule">The fluent validation <see cref="PropertyRule"/> that is being mapped.</param>
        /// <param name="validator">The <see cref="IPropertyValidator"/> of the rule.</param>
        public FluentAdapter(string ruleType, PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator)
        {
            this.ruleType = ruleType;
        }

        /// <summary>
        /// Get the <see cref="ValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationRule"/> instances.</returns>
        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ValidationRule(this.ruleType, FormatMessage, GetMemberNames());
        }
    }
}