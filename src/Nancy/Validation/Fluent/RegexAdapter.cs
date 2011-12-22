using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Validators;
using Nancy.Validation.Rules;

namespace Nancy.Validation.Fluent
{
    public class RegexAdapter : IFluentAdapter
    {
        private readonly string memberName;
        private readonly RegularExpressionValidator validator;

        public RegexAdapter(string memberName, RegularExpressionValidator validator)
        {
            this.memberName = memberName;
            this.validator = validator;
        }

        public IEnumerable<ValidationRule> GetRules()
        {
            //TODO: need to get error message out...
            yield return new RegexValidationRule(s => s, new[] { this.memberName }, this.validator.Expression);
        }


    }
}
