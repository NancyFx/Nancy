using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Validation.Fluent
{
    public interface IFluentAdapter
    {
        IEnumerable<ValidationRule> GetRules();
    }
}