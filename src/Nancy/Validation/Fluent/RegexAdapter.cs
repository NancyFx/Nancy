using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Validators;
using Nancy.Validation.Rules;
using FluentValidation.Internal;

namespace Nancy.Validation.Fluent
{
    public class RegexAdapter : AdapterBase
    {
        private readonly string pattern;

        public RegexAdapter(PropertyRule rule, RegularExpressionValidator validator)
            : base(rule, validator)
        {
            this.pattern = validator.Expression;
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new RegexValidationRule(FormatMessage, GetMemberNames(), this.pattern);
        }


    }
}
