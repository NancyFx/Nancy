using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Validators;
using Nancy.Validation.Rules;

namespace Nancy.Validation.Fluent
{
    public class FluentAdapter : IFluentAdapter
    {
        private readonly string ruleType;
        private readonly string memberName;
        private readonly IPropertyValidator validator;

        public FluentAdapter(string ruleType, string memberName, IPropertyValidator validator)
        {
            this.ruleType = ruleType;
            this.memberName = memberName;
            this.validator = validator;
        }

        public virtual IEnumerable<ValidationRule> GetRules()
        {
            //TODO: need to get error message out...
            yield return new ValidationRule(this.ruleType, s => s, new[] { this.memberName });
        }

    }
}