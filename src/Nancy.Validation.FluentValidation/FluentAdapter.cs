namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

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
        /// <param name="validator">The <see cref="PropertyRule"/> of the rule.</param>
        public FluentAdapter(string ruleType, PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator)
        {
            this.ruleType = ruleType;
        }

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new ModelValidationRule(this.ruleType, FormatMessage, GetMemberNames());
        }
    }
}