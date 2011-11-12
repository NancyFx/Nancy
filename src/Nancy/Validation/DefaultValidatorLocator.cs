namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;

    /// <summary>
    /// The default Nancy implementation of IValidatorLocator.
    /// </summary>
    public class DefaultValidatorLocator : IValidatorLocator
    {
        private readonly ConcurrentDictionary<Type, IValidator> cachedValidators;
        private readonly IEnumerable<IValidatorFactory> factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidatorLocator"/> class.
        /// </summary>
        /// <param name="factories">The factories.</param>
        public DefaultValidatorLocator(IEnumerable<IValidatorFactory> factories)
        {
            this.cachedValidators = new ConcurrentDictionary<Type, IValidator>();
            this.factories = factories ?? new List<IValidatorFactory>();
        }

        /// <summary>
        /// Gets a validator for a given type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns>
        /// IValidator instance or null if none found.
        /// </returns>
        public IValidator GetValidatorForType(Type type)
        {
            return cachedValidators.GetOrAdd(type, CreateValidator);
        }

        private IValidator CreateValidator(Type type)
        {
            var validators = factories
                .Select(f => f.Create(type))
                .Where(v => v != null)
                .ToList();

            if(validators.Count == 0)
                return null;

            if (validators.Count == 1)
                return validators[0];

            return new CompositeValidator(validators);
        }
    }
}