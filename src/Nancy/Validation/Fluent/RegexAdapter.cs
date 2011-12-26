namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using Nancy.Validation.Rules;
    using FluentValidation.Internal;

    /// <summary>
    /// Adapter between the Fluent Validation <see cref="IRegularExpressionValidator"/> and the Nancy validation rules.
    /// </summary>
    public class RegexAdapter : AdapterBase<IRegularExpressionValidator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexAdapter"/> class for the specified
        /// <paramref name="rule"/> and <paramref name="validator"/>.
        /// </summary>
        /// <param name="rule">The fluent validation <see cref="PropertyRule"/> that is being mapped.</param>
        /// <param name="validator">The <see cref="IPropertyValidator"/> of the rule.</param>
        public RegexAdapter(PropertyRule rule, IRegularExpressionValidator validator)
            : base(rule, validator)
        {
        }

        /// <summary>
        /// Get the <see cref="ValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationRule"/> instances.</returns>
        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new RegexValidationRule(FormatMessage, GetMemberNames(), this.Validator.Expression);
        }
    }
}
