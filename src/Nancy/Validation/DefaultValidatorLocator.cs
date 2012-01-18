namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;

    /// <summary>
    /// The default Nancy implementation of IValidatorLocator.
    /// </summary>
    public class DefaultValidatorLocator : IModelValidatorLocator
    {
        private readonly ConcurrentDictionary<Type, IModelValidator> cachedValidators;
        private readonly IEnumerable<IModelValidatorFactory> factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidatorLocator"/> class.
        /// </summary>
        /// <param name="factories">The factories.</param>
        public DefaultValidatorLocator(IEnumerable<IModelValidatorFactory> factories)
        {
            this.cachedValidators = 
                new ConcurrentDictionary<Type, IModelValidator>();

            this.factories = factories ?? Enumerable.Empty<IModelValidatorFactory>();
        }

        /// <summary>
        /// Gets a validator for a given type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns>An <see cref="IModelValidator"/> instance or <see langword="null"/> if none found.</returns>
        public IModelValidator GetValidatorForType(Type type)
        {
            if (!this.factories.Any())
            {
                throw new ModelValidationException("No model validator factory could be located.");
            }

            return cachedValidators.GetOrAdd(type, CreateValidator);
        }

        private IModelValidator CreateValidator(Type type)
        {
            var validators = factories
                .Select(f => f.Create(type))
                .Where(v => v != null)
                .ToList();

            if(validators.Count == 0)
            {
                return null;
            }

            return validators.Count == 1 ? 
                validators[0] : 
                new CompositeValidator(validators);
        }
    }
}