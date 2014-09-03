namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A description of the rules a validator provides.
    /// </summary>
    public class ModelValidationDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationDescriptor"/> class.
        /// </summary>
        /// <param name="rules">The rules that describes the model.</param>
        /// <param name="modelType">The type of the model that the rules are defined for.</param>
        public ModelValidationDescriptor(IEnumerable<ModelValidationRule> rules, Type modelType)
            : this(GetModelValidationRuleDictionary(rules), modelType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationDescriptor"/> class.
        /// </summary>
        /// <param name="rules">The rules that describes the model, grouped by member name.</param>
        /// <param name="modelType">The type of the model that the rules are defined for.</param>
        public ModelValidationDescriptor(IDictionary<string, IList<ModelValidationRule>> rules, Type modelType)
        {
            this.Rules = rules;
            this.ModelType = modelType;
        }

        /// <summary>
        /// The type of the model that is being described.
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains <see cref="ModelValidationRule"/> instances grouped by property name.</value>
        public IDictionary<string, IList<ModelValidationRule>> Rules { get; private set; }

        private static IDictionary<string, IList<ModelValidationRule>> GetModelValidationRuleDictionary(IEnumerable<ModelValidationRule> rules)
        {
            var results =
                new Dictionary<string, IList<ModelValidationRule>>(StringComparer.OrdinalIgnoreCase);

            if (rules == null)
            {
                return results;
            }

            foreach (var rule in rules)
            {
                foreach (var name in rule.MemberNames)
                {
                    if (!results.ContainsKey(name))
                    {
                        results.Add(name, new List<ModelValidationRule>());
                    }

                    results[name].Add(rule);
                }
            }

            return results;
        }
    }
}