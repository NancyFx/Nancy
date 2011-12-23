namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;

    public interface IFluentAdapter
    {
        IEnumerable<ValidationRule> GetRules();
    }
}