namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates and <see cref="IModelValidator"/> for DataAnnotations.
    /// </summary>
    public class DataAnnotationsValidatorFactory : IModelValidatorFactory
    {
        private readonly IPropertyValidatorFactory factory;
        private readonly IValidatableObjectAdapter validatableObjectAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="IPropertyValidatorFactory"/> instance that should be used by the factory.</param>
        /// <param name="validatableObjectAdapter">The <see cref="validatableObjectAdapter"/> instance that should be used by the factory.</param>
        public DataAnnotationsValidatorFactory(IPropertyValidatorFactory factory, IValidatableObjectAdapter validatableObjectAdapter)
        {
            this.factory = factory;
            this.validatableObjectAdapter = validatableObjectAdapter;
        }

        /// <summary>
        /// Creates a data annotations <see cref="IModelValidator"/> instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IModelValidator"/> instance. If no data annotation rules were found for the specified <paramref name="type"/> then <see langword="null"/> is returned.</returns>
        public IModelValidator Create(Type type) 
        {
            var validator = 
                new DataAnnotationsValidator(type, this.factory, this.validatableObjectAdapter);

            return validator.Description.Rules.Any() ?
                validator :
                null;
        }
    }
}