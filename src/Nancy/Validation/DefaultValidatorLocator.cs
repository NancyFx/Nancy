namespace Nancy.Validation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
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
                throw new ModelValidationException("No model validator factory could be located. Please ensure that you have an appropriate validation package installed, such as one of the Nancy.Validation packages.");
            }

            return cachedValidators.GetOrAdd(type, CreateValidator);
        }

        private IModelValidator CreateValidator(Type type)
        {
            var validators = this.factories
                .Select(factory => factory.Create(type))
                .Where(validator => validator != null)
                .ToArray();

            if(!validators.Any())
            {
                return null;
            }

            return (validators.Length == 1) ?
                validators[0] :
                new CompositeValidator(validators, type);
        }
    }
}