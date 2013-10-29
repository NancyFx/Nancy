namespace Nancy.Validation.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::FluentValidation;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Results;
    using global::FluentValidation.Validators;

    /// <summary>
    /// The default Fluent Validation implementation of <see cref="IValidator"/>.
    /// </summary>
    public class FluentValidationValidator : IModelValidator
    {
        private readonly IValidator validator;
        private readonly IFluentAdapterFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationValidator"/> class for the
        /// specified <see cref="IValidator"/>.
        /// </summary>
        /// <param name="validator">The Fluent Validation validator that should be used.</param>
        /// <param name="factory"> </param>
        public FluentValidationValidator(IValidator validator, IFluentAdapterFactory factory)
        {
            this.validator = validator;
            this.factory = factory;
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        /// <value>A <see cref="ModelValidationDescriptor"/> instance.</value>
        public ModelValidationDescriptor Description
        {
            get { return CreateDescriptor(); }
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance that is being validated.</param>
        /// <returns>A ValidationResult with the result of the validation.</returns>
        public ModelValidationResult Validate(object instance)
        {
            var result =
                this.validator.Validate(instance);

            var errors =
                GetErrors(result);

            return new ModelValidationResult(errors);
        }

        private ModelValidationDescriptor CreateDescriptor()
        {
            var fluentDescriptor =
                this.validator.CreateDescriptor();

            var rules = new List<ModelValidationRule>();

            var membersWithValidators = 
                fluentDescriptor.GetMembersWithValidators();

            foreach (var memberWithValidators in membersWithValidators)
            {
                var fluentRules = fluentDescriptor
                    .GetRulesForMember(memberWithValidators.Key)
                    .OfType<PropertyRule>();

                foreach (var rule in fluentRules)
                {
                    foreach (var v in rule.Validators)
                    {
                        rules.AddRange(GetValidationRule(rule, v));
                    }
                }
            }

            return new ModelValidationDescriptor(rules);
        }

        private static IEnumerable<ModelValidationError> GetErrors(ValidationResult results)
        {
            return results.IsValid ? 
                Enumerable.Empty<ModelValidationError>() :
                results.Errors.Select(error => new ModelValidationError(new[] { error.PropertyName }, s => error.ErrorMessage));
        }

        private IEnumerable<ModelValidationRule> GetValidationRule(PropertyRule rule, IPropertyValidator propertyValidator)
        {
            return this.factory.Create(rule, propertyValidator).GetRules(rule, propertyValidator);
        }
    }
}