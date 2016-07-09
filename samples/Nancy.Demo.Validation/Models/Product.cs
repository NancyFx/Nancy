namespace Nancy.Demo.Validation.Models
{
    using System;
    using System.Collections.Generic;

    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Validators;

    using Nancy.Validation;
    using Nancy.Validation.FluentValidation;

    public class Product
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }

    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(product => product.Name).NotEmpty();
            RuleFor(product => product.Name).Length(1, 10).OddLength();
            RuleFor(product => product.Name).Matches("[A-Z]*");
            RuleFor(product => product.Name).EmailAddress();

            RuleFor(product => product.Price).ExclusiveBetween(10, 15);
            RuleFor(product => product.Price).InclusiveBetween(10, 15);
            RuleFor(product => product.Price).Equal(5);
        }
    }

    public class OddLengthStringValidator : PropertyValidator
    {
        public OddLengthStringValidator()
            : base("'{PropertyName}' has to be of odd length.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var value =
                context.PropertyValue as string;

            if (value == null)
            {
                return false;
            }

            return (value.Length % 2 != 0);
        }
    }

    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> OddLength<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OddLengthStringValidator());
        }
    }

    public class OddLengthStringValidatorAdapter : AdapterBase
    {
        public override bool CanHandle(IPropertyValidator validator)
        {
            return validator is OddLengthStringValidator;
        }

        public override IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator)
        {
            yield return new OddLengthStringRule(
                base.FormatMessage(rule, validator),
                base.GetMemberNames(rule));
        }
    }

    public class OddLengthStringRule : ModelValidationRule
    {
        public OddLengthStringRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames)
            : base("OddLengthString", errorMessageFormatter, memberNames)
        {
        }
    }
}