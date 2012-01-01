namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using Nancy.Validation.Rules;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Adapter between the Fluent Validation <see cref="INotEmptyValidator"/> and the Nancy validation rules.
    /// </summary>
    public class NotEmptyAdapter : AdapterBase<INotEmptyValidator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyAdapter"/> class for the specified
        /// <paramref name="rule"/> and <paramref name="validator"/>.
        /// </summary>
        /// <param name="rule">The fluent validation <see cref="PropertyRule"/> that is being mapped.</param>
        /// <param name="validator">The <see cref="PropertyRule"/> of the rule.</param>
        public NotEmptyAdapter(PropertyRule rule, INotEmptyValidator validator)
            : base(rule, validator)
        {
        }

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new NotEmptyValidationRule(FormatMessage, GetMemberNames());
        }
    }
}