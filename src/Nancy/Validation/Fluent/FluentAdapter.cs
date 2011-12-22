using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Validators;
using Nancy.Validation.Rules;
using FluentValidation.Internal;

namespace Nancy.Validation.Fluent
{
    public class FluentAdapter : AdapterBase
    {
        private readonly string ruleType;

        public FluentAdapter(string ruleType, PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator)
        {
            this.ruleType = ruleType;
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ValidationRule(this.ruleType, FormatMessage, GetMemberNames());
        }
    }
}